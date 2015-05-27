package com.yahoo.inmind.middleware.control;

import android.app.Activity;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;

import com.google.common.util.concurrent.ListeningExecutorService;
import com.google.common.util.concurrent.MoreExecutors;
import com.yahoo.inmind.view.browser.BaseBrowser;
import com.yahoo.inmind.view.browser.LoginBrowser;
import com.yahoo.inmind.control.events.ExpandArticleEvent;
import com.yahoo.inmind.control.events.GoToArticleEvent;
import com.yahoo.inmind.control.events.RequestFetchNewsEvent;
import com.yahoo.inmind.middleware.events.MBRequest;
import com.yahoo.inmind.middleware.events.news.NewsResponseEvent;
import com.yahoo.inmind.middleware.events.news.NewsUpdateEvent;
import com.yahoo.inmind.model.vo.FilterVO;
import com.yahoo.inmind.control.news.NewsArticleVector;
import com.yahoo.inmind.model.vo.NewsArticle;
import com.yahoo.inmind.model.slingstone.UserProfile;
import com.yahoo.inmind.control.reader.ReaderController;
import com.yahoo.inmind.view.reader.ReaderMainActivity;
import com.yahoo.inmind.control.util.Constants;
import com.yahoo.inmind.control.util.Util;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Properties;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class NewsService extends Service {

    private Context mApp;
    private Properties properties;

    /** Binder given to clients */
    private final IBinder mBinder = new NewsBinder();

    /** Concurrency */
    private final int MAX_THREADS = 10;
    private ExecutorService executorService;
    private ListeningExecutorService pool;

    /** Communication **/
    private MessageBroker mb;

    /** Controllers **/
    private ReaderController reader;
    private HashMap<Integer, MBRequest> requests;

    /** update news automatically **/
    private Timer timer;
    private static long mUpdateTime;
    private static long mRefreshInterval; //seconds
    private static boolean isTimerActivated = false;
    private NewsArticle firstItem;



    // ****************************** SERVICE'S LIFE CYCLE *****************************************


    /**
     * Class used for the client Binder.  Because we know this service always
     * runs in the same process as its clients, we don't need to deal with IPC.
     */
    public class NewsBinder extends Binder {

        NewsService getService() {
            // Return this instance of NewsService so clients can call public methods
            return NewsService.this;
        }
    }
    @Override
    public IBinder onBind(Intent intent) {
        reader = ReaderController.getController();
        mb = MessageBroker.getInstance( mApp );
        mApp = getApplicationContext();
        initialize(getApplicationContext());
        //FIXME: load initial set of news is required?
//        MBRequest request = new MBRequest( Constants.MSG_GET_NEWS_ITEMS);
//        request.put( Constants.FLAG_SEND_EVENT, false );
//        getNewsItems( request );
        return mBinder;
    }

    @Override
    public void onDestroy(){
        timer.cancel();
        executorService.shutdown();
        pool.shutdown();
    }

    public void initialize( Context app ){
        try {
            reader.getSlingstone(getApplicationContext());
            executorService = Executors.newFixedThreadPool(MAX_THREADS);
            pool = MoreExecutors.listeningDecorator(executorService);
            requests = new HashMap<>();
            timer = new Timer();
            properties = Util.loadConfigAssets(app, "midd_config.properties");
            String value = properties.getProperty("newsRefreshTime");
            mRefreshInterval = value == null ? null : Long.parseLong(value);
            reader.setmRefreshInterval(mRefreshInterval);
            value = properties.getProperty("newsUpdateTime");
            mUpdateTime = value == null ? null : Long.parseLong(value);
        }catch (Exception e){
            //nothing
        }
    }

    public void clean() {
        if( timer != null ) {
            timer.purge();
            timer.cancel();
        }
    }

    // ****************************** GETTERS AND SETTERS ******************************************

    public static void setRefreshTime(long refreshTime) {
        mRefreshInterval = refreshTime;
        ReaderController.getController().setmRefreshInterval(mRefreshInterval);
    }

    public static void setmUpdateTime(long mUpdateTime) {
        NewsService.mUpdateTime = mUpdateTime;
        isTimerActivated = false;
    }


    public HashMap<Integer, MBRequest> getRequests() {
        return requests;
    }

    // ****************************** SERVICE CALLS ************************************************


    /***
     * This method retrieves the most recent list of news articles. First it tries to get it from the
     * static context and if the list is empty it tries to get it from the the NewsResponseEvent sticky
     * event. Then, applies the corresponding filter (if ones)
     * @param request
     * @return
     */
    public NewsArticleVector getNewsItems( MBRequest request ) {
        NewsArticleVector list = Util.cloneList(NewsArticleVector.getInstance());
        if( list.isEmpty() && mb.getStickyEvent( new NewsResponseEvent().getClass() ) != null){
            list = mb.getStickyEvent( new NewsResponseEvent().getClass() ).getNews();
        }
        if( list.isEmpty() ){
            requestNews( request );
        }
        reader.setFilters( (ArrayList<FilterVO>) request.get(Constants.BUNDLE_FILTERS) );
        reader.applyFilter(list, false);
        return list;
    }

    /**
     * This method is manifold purposes: it retrieves the list of news articles and then shows it either in
     * the predefine news reader activity or in a customized activity
     * @param request
     */
    public void startNewsActivity( MBRequest request){
        if( request.get(Constants.BUNDLE_MODIFIED_NEWS) != null ){
            reader.setmNewsModified( (NewsArticleVector) request.get(Constants.BUNDLE_MODIFIED_NEWS));
        }

        Class clazz = null;
        if( request.get( Constants.BUNDLE_ACTIVITY_NAME) == null ){
            clazz = ReaderMainActivity.class;
        }else{
            try {
                clazz = Class.forName( (String)request.get(Constants.BUNDLE_ACTIVITY_NAME) );
            } catch (ClassNotFoundException e) {
                e.printStackTrace();
            }
        }
        Intent intent = new Intent( mApp, clazz );
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.putExtra(Constants.UI_LANDSCAPE_LAYOUT, (Integer) request.get(Constants.UI_LANDSCAPE_LAYOUT));
        intent.putExtra( Constants.UI_PORTRAIT_LAYOUT, (Integer)request.get(Constants.UI_PORTRAIT_LAYOUT) );
        intent.putExtra( Constants.UI_NEWS_RANK, (Integer)request.get( Constants.UI_NEWS_RANK ) );
        intent.putExtra( Constants.UI_NEWS_TITLE, (Integer)request.get( Constants.UI_NEWS_TITLE ) );
        intent.putExtra( Constants.UI_NEWS_SCORE, (Integer)request.get( Constants.UI_NEWS_SCORE ) );
        intent.putExtra( Constants.UI_NEWS_SUMMARY, (Integer)request.get( Constants.UI_NEWS_SUMMARY ) );
        intent.putExtra( Constants.UI_NEWS_FEAT, (Integer)request.get( Constants.UI_NEWS_FEAT ) );
        intent.putExtra( Constants.UI_NEWS_FEAT2, (Integer)request.get( Constants.UI_NEWS_FEAT2 ) );
        intent.putExtra( Constants.UI_NEWS_PUBLISHER, (Integer)request.get( Constants.UI_NEWS_PUBLISHER ) );
        intent.putExtra( Constants.UI_NEWS_REASON, (Integer)request.get( Constants.UI_NEWS_REASON ) );
        intent.putExtra( Constants.UI_NEWS_IMG, (Integer)request.get( Constants.UI_NEWS_IMG ) );
        intent.putExtra( Constants.UI_NEWS_SHARE_FB, (Integer)request.get( Constants.UI_NEWS_SHARE_FB ) );
        intent.putExtra( Constants.UI_NEWS_SHARE_TWITTER, (Integer)request.get( Constants.UI_NEWS_SHARE_TWITTER ) );
        intent.putExtra( Constants.UI_NEWS_SHARE_TMBLR, (Integer)request.get( Constants.UI_NEWS_SHARE_TMBLR ) );
        intent.putExtra(Constants.UI_NEWS_SHARE_MORE, (Integer) request.get(Constants.UI_NEWS_SHARE_MORE));
        intent.putExtra( Constants.FLAG_REFRESH, (Boolean) request.get(Constants.FLAG_REFRESH));

        if( request.get(Constants.BUNDLE_FILTERS) != null ){
            reader.setFilters((ArrayList<FilterVO>) request.get(Constants.BUNDLE_FILTERS));
            reader.isApplyFilter = true;
        }

        if( request.get(Constants.CONFIG_NEWS_RANKING_OPTION ) != null ){
            ReaderController.setRankingOption( (Integer) request.get(Constants.CONFIG_NEWS_RANKING_OPTION ) );
        }

        checkTimer();
        mApp.startActivity(intent);
    }


    /**
     * This method retrieves a list of news articles (the result is gotten in the event handler
     * onEvent( NewsResponseEvent event). If the current list of news articles is not updated then
     * it requests an update to the yahoo's news server (slingstone) otherwise returns the current
     * list of news articles.
     * @param request
     */
    public void loadNewsItems(MBRequest request) {
        requests.put(request.hashCode(), request);
        if (reader.isReload() || ( request.get(Constants.FLAG_FORCE_RELOAD) != null &&
                (Boolean) request.get(Constants.FLAG_FORCE_RELOAD)) ) {
            requestNews(request);
        } else {
            getNewsItemList(request.hashCode(), request);
        }
        checkTimer();
    }

    public UserProfile requestUserProfile(MBRequest request) {
        return reader.getUserProfile( );
    }

    private void requestNews(MBRequest request){
        if( reader.isInitialized ){
            RequestFetchNewsEvent event = new RequestFetchNewsEvent(request.hashCode());
            event.setArticleId( (Integer)request.get( Constants.BUNDLE_ARTICLE_ID ) );
            mb.send(event);
        }else{
            reader.createNewsFuture();
            pool.submit(new NewsAsyncFunction( request ));
        }
    }


    /**
     * This method retrieves the most recent list of news articles and send it to the UI through
     * the NewsResponseEvent event
     * @param messageId
     * @param request
     */
    public void getNewsItemList( int messageId, MBRequest request ) {
        MBRequest req = requests.remove(messageId);
        if( req != null ) {
            NewsArticleVector list = getNewsItems(request);
            // we don't want to send the response event when we are not waiting for a response,
            // otherwise events would be handled by the consumer without previous request
            Boolean shouldSend = (Boolean) req.get(Constants.FLAG_SEND_EVENT);
            if ((shouldSend == null) || (shouldSend != null && shouldSend == true)) {
                NewsResponseEvent event;
                if (list != null && !list.isEmpty()) {
                    if (req.get(Constants.FLAG_RETURN_JSON) != null &&
                            (Boolean) req.get(Constants.FLAG_RETURN_JSON) == true) {
                        event = new NewsResponseEvent((Integer) req.get(Constants.QUALIFIER_NEWS),
                                list.toJson());
                    } else {
                        Integer qualifier = req.get(Constants.QUALIFIER_NEWS) == null ? 0
                                : (Integer) req.get(Constants.QUALIFIER_NEWS);
                        event = new NewsResponseEvent(qualifier, list);
                    }
                    firstItem = list.get(0);
                } else {
                    event = new NewsResponseEvent();
                }
                event.setMbRequestId(request.hashCode());
                mb.postSticky(event); //Send msg to notify UI to updatelist
            }
        }
    }

    /**
     * This method sends a news update notification to the UI. This is activated when the service
     * identified a change on the list of news in the server
     */
    public void getNewsUpdate( MBRequest request ) {
        //request.setRequestId( Constants.MSG_GET_NEWS_ITEMS );
        NewsArticleVector list = getNewsItems( request );
        if( firstItem != null && firstItem.getTitle().equals( list.get(0).getTitle() ) == false ) {
            NewsUpdateEvent event = new NewsUpdateEvent(list);
            firstItem = list.get(0);
            mb.postSticky(event);
        }
    }


    /**
     * This method opens the login activity. The results of this action are retrieved to the bound
     * activity (which comes into the cache memory) by calling the onActivityResult() method.
     * @param request
     */
    public void login(MBRequest request) {
        Intent intent = new Intent(mApp, LoginBrowser.class);
        intent.putExtra(BaseBrowser.LAUCH_BROWSER_URL, LoginBrowser.loginUrl);
        Activity parent = (Activity) request.get( Constants.CONTENT);
        parent.startActivityForResult(intent, (Integer)request.get( Constants.RESULTS_LOGIN));
    }


    public NewsArticleVector applyFilter(MBRequest request, NewsArticleVector newsItems) {
        if( newsItems == null ){
            newsItems = getNewsItems( request );
            if( newsItems == null ){
                return NewsArticleVector.getInstance();
            }
        }
        reader.setFilters( (ArrayList<FilterVO>)request.get(Constants.BUNDLE_FILTERS) );
        reader.isApplyFilter = true;
        if( newsItems == null  || newsItems.isEmpty() ){
            getNewsItemList( request.hashCode(), request );
        }
        return NewsArticleVector.wrap(reader.applyFilter(newsItems, true));
    }

    public static void setNewsListSize( int size ){
        ReaderController.getController().setNewsSize(size);
    }

    public void showArticle(MBRequest mbRequest) {
        GoToArticleEvent event = new GoToArticleEvent();
        if( mbRequest.get( Constants.BUNDLE_ARTICLE_ID ) == null ){
            event.setIdx( reader.getCurrentArticle() );
        }else{
            if( mbRequest.get( Constants.BUNDLE_ARTICLE_ID ) instanceof String ){
                String position = (String) mbRequest.get( Constants.BUNDLE_ARTICLE_ID );
                if( position.equals( Constants.ARTICLE_NEXT_POSITION ) ) {
                    reader.setCurrentArticle( reader.getCurrentArticle() + 1);
                    event.setIdx( reader.getCurrentArticle() );
                } else if( position.equals( Constants.ARTICLE_PREVIOUS_POSITION ) ) {
                    reader.setCurrentArticle( reader.getCurrentArticle() - 1);
                    event.setIdx( reader.getCurrentArticle() );
                }
            }else if( mbRequest.get( Constants.BUNDLE_ARTICLE_ID ) instanceof Integer ){
                Integer position = (Integer) mbRequest.get( Constants.BUNDLE_ARTICLE_ID);
                event.setIdx( position );
            }
        }
        mb.send( event );
    }

    public void expandArticle(MBRequest mbRequest) {
        ExpandArticleEvent event = new ExpandArticleEvent();
        Integer position = (Integer) mbRequest.get( Constants.BUNDLE_ARTICLE_ID );
        if( position != null ) {
            event.setIdx( position );
        }else{
            event.setIdx( reader.getCurrentArticle() );
        }
        mb.send( event );
    }

    public Integer getArticle(MBRequest mbRequest) {
       return reader.getCurrentArticle();
    }


    // ****************************** HELPER CLASSES AND METHODS ***********************************

    private final class NewsAsyncFunction implements Callable<Void> {

        private MBRequest request;

        public NewsAsyncFunction(MBRequest request){
            this.request = request;
        }

        public Void call() throws Exception {
            requests.put( request.hashCode(), request );
            reader.initialize(null, null, request.hashCode());
            return null;
        }
    }

    /**
     * This component checks whether there is an updated list of news articles or not.
     */
    private final class NewsTimerTask extends TimerTask{
        @Override
        public void run() {
            MBRequest request = new MBRequest( Constants.MSG_UPDATE_NEWS );
            request.put( Constants.FLAG_UPDATE_NEWS, true );
            requestNews( request );
        }
    }

    private void checkTimer(){
        if( isTimerActivated == false ){
            timer.schedule( new NewsTimerTask(), mUpdateTime, mUpdateTime );
            isTimerActivated = true;
        }
    }
}
