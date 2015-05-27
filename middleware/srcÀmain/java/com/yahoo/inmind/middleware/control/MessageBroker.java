package com.yahoo.inmind.middleware.control;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.IBinder;
import android.util.Log;
import android.util.LruCache;
import android.widget.Toast;

import com.yahoo.inmind.control.events.BaseEvent;
import com.yahoo.inmind.control.events.ResponseFetchNewsEvent;
import com.yahoo.inmind.middleware.events.AudioRecordEvent;
import com.yahoo.inmind.middleware.events.MBRequest;
import com.yahoo.inmind.control.news.NewsArticleVector;
import com.yahoo.inmind.model.slingstone.UserProfile;
import com.yahoo.inmind.control.reader.ReaderController;
import com.yahoo.inmind.control.util.Constants;

import java.util.HashMap;

import de.greenrobot.event.EventBus;


/**
 * Created by oscarr on 12/5/14.
 */
public class MessageBroker {

    public final static String TAG = "inmind";

    /** Publish/subscribe */
    private static EventBus eventBus;
    private HashMap<Integer, SubscriberEvent> subscribers;

    /** Flag indicating whether we have called bind on the service. */
    private boolean newsServBound;
    private NewsService newsService;

    private static MessageBroker mMB;
    private static Context mContext;

    /** Cache Memory **/
    private static LruCache<Object, Object> cache;


    // ****************************** MB'S LIFECYCLE *******************

    private MessageBroker() {
        // we need to bind to the ews service.
        // this will be done only once.
        try {
            mContext.bindService(new Intent(mContext, NewsService.class), mConnection,
                    Context.BIND_AUTO_CREATE);
        }catch (Exception e){
            //nothing
        }

        ReaderController.getController(mContext);
        eventBus = EventBus.getDefault();
        cache = new LruCache<>( 500 );
        subscribers = new HashMap<>();
    }

    /**
     * Singleton
     * @return
     */
    public static MessageBroker getInstance( Context app ){
        if( mMB == null ){
            if ( app == null ){
                Log.e( TAG, "There is no Context to bind the component.");
            }else{
                mContext = app;
                mMB = new MessageBroker();
            }
        }
        return mMB;
    }

    public void destroy(){
        if( newsService != null ) {
            newsService.clean();
        }
        // Unbind from the service
        if (newsServBound) {
            mContext.unbindService(mConnection);
            newsServBound = false;
        }
        newsService = null;
        mMB = null;
        mContext = null;
        cache.evictAll();
        subscribers = null;
        System.gc();
    }


    // ****************************** HELPER CLASSES ***********************************************

    /** Defines callbacks for service binding, passed to bindService() */
    private ServiceConnection mConnection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName className,
                                       IBinder service) {
            // We've bound to NewsService, cast the IBinder and get NewsService instance
            NewsService.NewsBinder binder = (NewsService.NewsBinder) service;
            newsService = binder.getService();
            newsServBound = true;
        }

        @Override
        public void onServiceDisconnected(ComponentName arg0) {
            newsServBound = false;
        }
    };

    class SubscriberEvent{
        private Class event;
        private Object subscriber;

        public SubscriberEvent( Class event, Object subscriber ){
            this.event = event;
            this.subscriber = subscriber;
        }

        public Class getEvent() {
            return event;
        }

        public void setEvent(Class event) {
            this.event = event;
        }

        public Object getSubscriber() {
            return subscriber;
        }

        public void setSubscriber(Object subscriber) {
            this.subscriber = subscriber;
        }
    }

    // ****************************** REQUESTS HANDLERS ********************************************

    /**
     * Use this method to send asynchronous requests to the backend (services, background tasks, etc.)
     * If your request returns a result, then it should be handled by an event handler defined outside
     * the Message Broker (usually in the parent class that invoked the message broker's send method)
     * The result of this request is delivered to all the subscribers of the resulting event
     * @param request
     */
    public void send(Object request) {
        if (request instanceof MBRequest) {
            MBRequest mbRequest = (MBRequest) request;
            final int requestId = mbRequest.getRequestId();
            switch (requestId) {
                case Constants.MSG_LAUNCH_BASE_NEWS_ACTIVITY:
                    launchNewsActivity(mbRequest);
                    break;
                case Constants.MSG_LAUNCH_EXT_NEWS_ACTIVITY:
                    launchNewsActivity(mbRequest);
                    break;
                case Constants.MSG_REQUEST_NEWS_ITEMS:
                    requestNewsItems(mbRequest);
                    break;
                case Constants.MSG_SHOW_MODIFIED_NEWS:
                    launchNewsActivity(mbRequest);
                    break;
                case Constants.MSG_LOGIN:
                    login( mbRequest );
                    break;
                case Constants.MSG_SHOW_ARTICLE:
                    showArticle( mbRequest );
                    break;
                case Constants.MSG_EXPAND_ARTICLE:
                    expandArticle(mbRequest);
                    break;
                case Constants.MSG_LAUNCH_ACTIVITY:
                    launchActivity( mbRequest );
                    break;
                case Constants.MSG_SHOW_NEXT_ARTICLE:
                    showNextArticle( mbRequest );
                    break;
                case Constants.MSG_SHOW_CURRENT_ARTICLE:
                    showCurrentArticle( mbRequest );
                    break;
                case Constants.MSG_SHOW_PREVIOUS_ARTICLE:
                    showPreviousArticle( mbRequest );
                    break;
                case Constants.MSG_START_AUDIO_RECORD:
                    recordAudio(mbRequest);
                    break;
                case Constants.MSG_STOP_AUDIO_RECORD:
                    stopRecordAudio(mbRequest);
                    break;
                case Constants.MSG_UPLOAD_TO_SERVER:
                    uploadToServer(mbRequest);
                    break;
                default:
                    Toast.makeText( mContext, "The MBRequest object has a non valid id",
                            Toast.LENGTH_LONG).show();
                    break;
            }
        } else {
            post(request);
        }
    }



    /**
     * This is an asynchronous request like {@link #send(Object)} but the difference is that
     * this method requires that the response event be delivered to only the caller subscriber,
     * not to all the objects that are subscribed to this event.
     * @param subscriber This is the subscriber object that will receive the response
     * @param request This is the request message
     * @param event this is the event that the subscriber will handle on its onEvent(event) method
     */
    public void sendAndReceive(Object subscriber, Object request, Class event){
        SubscriberEvent subsEvent = new SubscriberEvent( event, subscriber );
        subscribers.put( request.hashCode(), subsEvent );
        send( request );
    }


    /**
     * This method checks whether the event has a valid request id. If so, it looks up the
     * corresponding subscriber for this event and set it into the event object
     * @param event
     */
    private void setSubscriber( Object event ){
        if( event instanceof BaseEvent ){
            if( ((BaseEvent) event).getMbRequestId() != null ) {
                int id = ((BaseEvent) event).getMbRequestId();
                SubscriberEvent subsEvent = subscribers.remove(id);
                if (subsEvent != null && subsEvent.getEvent() == event.getClass()) {
                    Object subscriber = subsEvent.getSubscriber();
                    ((BaseEvent) event).setSubscriber(subscriber);
                }
            }
        }
    }


    /**
     * Use this method to send a synchronous request and get an immediate result. The request should
     * not take too much processing time, so it warranties not to block the main thread (UI thread).
     * @param request
     * @return
     */
    public Object get( MBRequest request ){
        Object result = null;
        switch ( request.getRequestId() ){
            case Constants.MSG_GET_USER_PROFILE:
                result = getUserProfile( request );
                break;
            case Constants.MSG_GET_NEWS_ITEMS:
                result = getNewsItems( request );
                break;
            case Constants.MSG_APPLY_FILTERS:
                result = applyFilters(request);
                break;
            case Constants.MSG_GET_ARTICLE_POSITION:
                result = getArticle( request );
                break;
            default:
                Toast.makeText( mContext, "The MBRequest object has a non valid id",
                        Toast.LENGTH_LONG).show();
                break;
        }
        return result;
    }

    /**
     * Use this method to subscribe to messages that are published by other components. Using this
     * method will allow you implement overloaded versions of onEvent method (a event handler method)
     * @param subscriber
     */
    public void subscribe( Object subscriber ) {
        if( eventBus.isRegistered( subscriber ) == false ) {
            eventBus.register(subscriber);
        }
    }


    /**
     * This method subscribes the subscriber object to a specific event updates. However, unless
     * {@link #subscribe(Object)}, this method set the update process to a hold-on state until
     * explicit notification from the subscriber of being ready to start receiving updates is sent.
     * @param subscriber
     * @param event
     */
    public void subscribe( Object subscriber, Class... event ) {
        //add a subscription exception to avoid receiving updates until the subscriber is ready
        for( Class e : event ) {
            eventBus.addSubscriptionException(subscriber, e);
        }
        if( eventBus.isRegistered( subscriber ) == false ) {
            eventBus.register(subscriber);
        }
    }



    /**
     * Use this method to unsubscribe to any kind of messages published by other components. This is
     * useful to improve the resource management, for instance, use this method when the activity
     * is paused (onPause method) and subscribe again when it resumes (onResume method)
     * @param subscriber
     */
    public void unSubscribe( Object subscriber ) {
        eventBus.unregister(subscriber);
    }


    public void unSubscribe( Object subscriber, Class event ){
        eventBus.addSubscriptionException( subscriber, event );
    }

    public void removeSubscriptionException( Object subscriber, Class event ){
        eventBus.removeSubscriptionException( subscriber, event );
    }


    /**
     * This method posts a sticky event, that is, the event is cached by the message broker so you
     * can get it anytime by using getSticky method. This is for internal use of the middleware.
     * @param event
     */
    public void postSticky( Object event ){
        setSubscriber( event );
        eventBus.postSticky( event );
    }

    private void post( Object event ){
        setSubscriber( event );
        eventBus.post( event );
    }

    /**
     * This method gets a specific event which has been cached by the message broker. This is for
     * internal use of the middleware.
     * @param eventType
     * @param <T>
     * @return
     */
    public <T> T getStickyEvent( Class<T> eventType ){
        return eventBus.getStickyEvent( eventType );
    }


    // ****************************** NEWS SERVICE CALLS *******************************************


    /**
     * @param mbRequest this MBRequest object should contain:
     *                  Constants.HTTP_REQUEST_SERVER_URL: the destination server URL
     *                  Constants.HTTP_REQUEST_BODY: body of the request. It could be:
     *                      - a File Object
     *                      - a byte[] array
     *                      - an InputStream
     *                      - a String (location of the resource to be uploaded in the phone's sdcard memory)
     *                      - a BitMap
     *                  Constants.HTTP_REQUEST_PARAMS: if no body is provided, then it uses the params
     *                  Constants.HTTP_REQUEST_CONNECTION_TIMEOUT: Connection time in miliseconds
     *                  Constants.IMG_COMPRESS_FORMAT: The format of the compressed image (e.g.,JPEG, PNG, etc.)
     *                  Constants.IMG_QUALITY: Hint to the compressor, 0-100. 0 meaning compress for small size,
     *                  100 meaning compress for max quality. Some formats, like PNG which is lossless,
     *                  will ignore the quality setting
     */
    private void uploadToServer(MBRequest mbRequest) {
        FileUploader.upload( mbRequest );
    }




    private void recordAudio(MBRequest mbRequest) {
        eventBus.removeSubscriptionException( mbRequest.get( Constants.SET_SUBSCRIBER ), AudioRecordEvent.class );
        AudioController.getInstance( (Integer) mbRequest.get(
                        Constants.SET_AUDIO_SAMPLE_RATE),
                (Integer) mbRequest.get( Constants.SET_AUDIO_CHANNEL_CONFIG ),
                (Integer) mbRequest.get( Constants.SET_AUDIO_ENCODING),
                (Integer) mbRequest.get( Constants.SET_AUDIO_BUFFER_ELEMENTS_TO_REC),
                (Integer) mbRequest.get( Constants.SET_AUDIO_BYTES_PER_ELEMENT),
                mMB,
                mbRequest.get( Constants.SET_SUBSCRIBER ));
    }

    private void stopRecordAudio(MBRequest request){
        unSubscribe( request.get( Constants.SET_SUBSCRIBER ), AudioRecordEvent.class );
        AudioController.getInstance( this ).unsubscribe( request.get( Constants.SET_SUBSCRIBER ) );
    }


    private void showPreviousArticle( MBRequest request ) {
        request.put( Constants.BUNDLE_ARTICLE_ID, Constants.ARTICLE_PREVIOUS_POSITION );
        this.newsService.showArticle(request);
    }

    private void showCurrentArticle( MBRequest request ) {
        this.newsService.showArticle( request );
    }

    private void showNextArticle( MBRequest request ) {
        request.put( Constants.BUNDLE_ARTICLE_ID, Constants.ARTICLE_NEXT_POSITION );
        this.newsService.showArticle( request );
    }

    private void launchNewsActivity( MBRequest request ) {
        newsService.startNewsActivity(request);
    }

    private void requestNewsItems( MBRequest request ) {
        newsService.loadNewsItems(request);
    }

    private NewsArticleVector getNewsItems( MBRequest request ) {
        return newsService.getNewsItems( request );
    }

    /**
     * This method can be used to change programmatically the time interval for the list of news
     * items to be refreshed from the Yahoo' news server. This time interval cn also be defined in
     * your midd_config.properties file
     * @param milis
     */
    public static void setNewsRefreshTime( long milis ){
        NewsService.setRefreshTime(milis);
    }

    /**
     * This method can be used to change programmatically the time interval for the list of news
     * items to be automatically updated from the Yahoo' news server. The difference between this
     * method and setNewsRefreshTime method is that the latter is a time interval used when you
     * explicitly request the list of news articles whereas the former is used by a timer task that
     * automatically retrieves the latest list of news articles. This time interval cn also be defined
     * in your midd_config.properties file
     * @param milis
     */
    private static void setNewsUpdateTime( long milis ){
        NewsService.setmUpdateTime(milis);
    }

    private static void setNewsSize(int newsSize) {
        NewsService.setNewsListSize( newsSize );
    }

    private void login( MBRequest request ){
        newsService.login(request);
    }

    private UserProfile getUserProfile( MBRequest request ){
        return newsService.requestUserProfile(request);
    }

    private NewsArticleVector applyFilters(MBRequest request) {
        return newsService.applyFilter( request,
                (NewsArticleVector)request.get( Constants.CONTENT_NEWS_LIST) );
    }

    public static void set(MBRequest mbRequest) {
        switch ( mbRequest.getRequestId() ){
            case Constants.SET_NEWS_LIST_SIZE:
                setNewsSize( (int) mbRequest.getValues()[0] );
                break;
            case Constants.SET_REFRESH_TIME:
                setNewsRefreshTime((long) mbRequest.getValues()[0]);
                break;
            case Constants.SET_UPDATE_TIME:
                setNewsUpdateTime( (long) mbRequest.getValues()[0] );
                break;
            default:
                break;
        }
    }

    private void showArticle(MBRequest mbRequest) {
        newsService.showArticle( mbRequest );
    }

    private void expandArticle(MBRequest mbRequest) {
        newsService.expandArticle( mbRequest );
    }

    private Integer getArticle(MBRequest mbRequest) {
        return newsService.getArticle( mbRequest );
    }

    // ****************************** CACHE MEMORY *************************************************

    /**
     * This method adds an object to the cache memory
     * @param object object to be added
     * @param id this is used to retrieve the object
     */
    public void addObjToCache(Object id, Object object) {
        synchronized ( cache ) {
            cache.put(id, object);
        }
    }

    /**
     * This method retrieves an object from the cache memory
     * @param id of the object
     * @param remove whether the object must be removed from the cache or not
     * @return
     */
    public Object getObjFromCache( Object id , boolean remove ){
        synchronized ( cache ) {
            if (remove) {
                return cache.remove(id);
            }
            return cache.get(id);
        }
    }



    // ****************************** EVENT HANDLERS ***********************************************

    /**
     * This event handler processes both the explicit request of the news list and the automatic
     * update of this list
     * @param event
     */
    public void onEventMainThread( ResponseFetchNewsEvent event ){
        Boolean updateNews = newsService.getRequests().get(event.getMbRequestId()) == null? null
                : (Boolean) newsService.getRequests().get(event.getMbRequestId()).
                    get(Constants.FLAG_UPDATE_NEWS);
        if( updateNews != null && updateNews == true ){
            newsService.getNewsUpdate(newsService.getRequests().remove(event.getMbRequestId()));
        }else{
            newsService.getNewsItemList(event.getMbRequestId(), newsService.getRequests().
                    get(event.getMbRequestId()));
        }
    }



    // ****************************** HELPERS CLASSES***********************************************
    private void launchActivity( MBRequest request ){
        try {
            Class clazz = null;
            if (request.get(Constants.BUNDLE_ACTIVITY_NAME) == null) {
                throw new Exception("The name of the activity doesn't exist. MB cannot create a new activity");
            } else {
                try {
                    clazz = Class.forName((String) request.get(Constants.BUNDLE_ACTIVITY_NAME));
                } catch (ClassNotFoundException e) {
                    e.printStackTrace();
                }
            }
            Intent intent = new Intent(mContext, clazz);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            mContext.startActivity( intent );
        }catch(Exception e){
            e.printStackTrace();
        }
    }


}
