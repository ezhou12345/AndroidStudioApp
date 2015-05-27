package com.yahoo.inmind.control.news;

import android.os.Environment;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.yahoo.inmind.control.events.RequestFetchNewsEvent;
import com.yahoo.inmind.control.reader.ReaderController;
import com.yahoo.inmind.model.vo.JsonItem;
import com.yahoo.inmind.model.vo.NewsArticle;
import com.yahoo.inmind.control.util.Util;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.PrintWriter;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

import de.greenrobot.event.EventBus;

/**
 * Created by oscarr on 12/11/14.
 */
public class NewsArticleVector extends ArrayList<NewsArticle>{

    public static NewsArticleVector instance;
    private static transient List<NewsArticle> list1; // re-ranked list from William's algorithm
    private static transient List<NewsArticle> list2; // re-ranked list from Emma's algorithm
    private static transient ArrayList<String> visitedArticles; // sub list which is shown to user
    private static transient ArrayList<NewsArticle> visitedList; // for Sneha
    private static transient Map<String, NewsArticle> mappings;
    private static transient Gson gson = new Gson();
    private static transient int endPosBatch;
    private static transient int numArtPerView = 1;
    private static transient int currentPos = 0;
    private static transient int startPosBatch = 0;

    /** number of articles to be shown before calling the personalization algorithms to re-rank the list.
     * Default value is 10, but this is taken from the news_config.properties file
     */
    private static transient int batchArticlesToShow = 10;
    private transient String stringRepresentation = "";

    private NewsArticleVector() {
        super();
    }

    private NewsArticleVector(List<NewsArticle> list, boolean loadParameters ) {
        super(list);
        synchronized( this ) {
            if (loadParameters) {
                initialize(list);
            }
        }
    }

    public synchronized static int getNumArtPerView(){
        return numArtPerView;
    }

    public synchronized static NewsArticleVector getArticlesPerView() {
        int start = startPosBatch;
        int end = start + numArtPerView;
        if( currentPos == end ) {
            startPosBatch = end;
        }
        return wrap(getInstance().subList( start, end ));
    }

    public static synchronized void initialize(List<NewsArticle> list) {
        release();
        endPosBatch = 0;
        startPosBatch = 0;
        batchArticlesToShow = Integer.valueOf(Util.loadConfigAssets(ReaderController.get(),
                "news_config.properties").getProperty("CONFIG_BATCH_ARTICLES_PERSONALIZATION"));
        numArtPerView = Integer.valueOf( Util.loadConfigAssets(ReaderController.get(),
                "news_config.properties").getProperty("CONFIG_NUM_ARTICLES_PER_VIEW"));
        mappings = new HashMap<>();
        visitedArticles = new ArrayList<>();
        visitedList = new ArrayList<>();
        currentPos = 0;
        createMappings(list);
    }

    public static synchronized void initialize(){
        int endPos = endPosBatch, startPos = startPosBatch;
        initialize( null );
        endPosBatch = endPos;
        startPosBatch = startPos;
    }


    private static synchronized void createMappings( List<NewsArticle> list ){
        if( list == null ){
            list = instance;
        }
        if( list != null ) {
            for (NewsArticle article : list) {
                mappings.put(article.getTitle(), article);
            }
        }
    }


    /***
     * Unlesss getInstance(), this method returns the whole set of news articles
     * @return
     */
    public synchronized static NewsArticleVector getInstance() {
        if (instance == null) {
            instance = new NewsArticleVector( Collections.synchronizedList(new NewsArticleVector()), false );
        }
        return instance;
    }


    /**
     * Creates a new NewsArticleVector which is not stored as static variable, that is, this is
     * not a singleton instance.
     * @return
     */
    public synchronized static NewsArticleVector wrap(List<NewsArticle> list){
        return new NewsArticleVector( list, false );
    }

    public static synchronized void replace( List list ) {
        if( instance != null ) {
            instance.clear();
        }
        instance = new NewsArticleVector( list, false );
        initialize( instance );
    }

    public static int getStartPosBatch() {
        return startPosBatch;
    }

    public static void setIsRecommendedBy(List<NewsArticle> list) {
        for( int i = 0; i < list.size(); i++ ){
            String key = list.get(i).getTitle();
            NewsArticle article = mappings.get(key);
            if( list == list1 ){
                article.setRecommendation1( true );
            } else if( list == list2 ){
                article.setRecommendation2( true );
            }
        }
    }


    public synchronized void clear(){
        super.clear();
        stringRepresentation = "";
    }

    public synchronized void add(JsonItem item){
        super.add((NewsArticle) item);
        stringRepresentation = "";
    }

    public synchronized NewsArticle get(int idx){
        return super.get(idx);
    }

    public synchronized NewsArticle remove(int idx){
        stringRepresentation = "";
        return super.remove(idx);
    }

    public synchronized boolean remove(NewsArticle art){
        stringRepresentation = "";
        return super.remove(art);
    }

    public synchronized String toJson(){
        if( stringRepresentation.equals("") && this == instance ){
            stringRepresentation = gson.toJson( this );
            return stringRepresentation;
        }
        return gson.toJson(this);
    }

    public synchronized static NewsArticleVector fromJson( String json, boolean loadParameters ){
        Type type = new TypeToken<List<NewsArticle>>() {}.getType();
        List<NewsArticle> list = gson.fromJson(json, type);
        return new NewsArticleVector( list, loadParameters );
    }


    public synchronized static ArrayList<JsonItem> getJsonItemlist( NewsArticleVector vector ){
        ArrayList<JsonItem> list = new ArrayList<>();
        for( NewsArticle article : vector ){
            list.add( article );
        }
        return list;
    }

    /**
     * Merge the results of 2 re-ranked lists
     * FIXME: replace this with a convenient algorithm
     */
    public synchronized static List mergeLists(){
        if( list1 != null && list2 != null && list1.size() == list2.size() ){
            HashMap<String, NewsArticle> map =  new HashMap<>();
            cleanRanks();
            computeRank( map, list1 );
            computeRank(map, list2);

            ArrayList<NewsArticle> temp = new ArrayList<>( map.values() );
            Collections.sort(temp, new Comparator<NewsArticle>() {
                @Override
                public int compare(NewsArticle lhs, NewsArticle rhs) {
                    return (int) (lhs.getRank() - rhs.getRank());
                }
            });
            return temp;
        }
        return null;
    }

    private synchronized static void computeRank( HashMap<String, NewsArticle> map, List<NewsArticle> list ){
        for( int i = 0; i < list.size(); i++ ){
            String key = list.get(i).getTitle();
            NewsArticle article = mappings.get(key);
            double value;
            try {
                value = article.getRank() + i;
            }catch(NullPointerException ex){
                article = list.get(i);
                value = article.getRank() + i;
            }
            article.setRank(value);
            map.put( key, article );
        }
    }

    private synchronized static void cleanRanks(){
        for( NewsArticle article : mappings.values() ){
            article.setRank( 0 );
        }
    }

    public synchronized static List<NewsArticle> getList1() {
        return list1;
    }

    public synchronized static void setList1(List<NewsArticle> list) {
        list1 = Util.clone( list );
    }

    public synchronized static List<NewsArticle> getList2() {
        return list2;
    }

    public synchronized static void setList2(List<NewsArticle> list) {
        list2 = Util.clone( list );
    }


    public synchronized static int getBatchArticlesToShow() {
        return batchArticlesToShow;
    }

    public synchronized static int getEndPosBatch() {
        return endPosBatch;
    }

    public synchronized static int getCurrentPos() {
        return currentPos;
    }

    public synchronized List<NewsArticle> getRemaining() {
        if( endPosBatch >= 0 && endPosBatch < size() ) {
            return subList(endPosBatch, size() );
        }
        return this;
    }

    public synchronized static int increaseCurrentPosition() {
        return ++currentPos;
    }

    public synchronized static int decreaseCurrentPosition() {
        return --currentPos;
    }

    public synchronized static void setVisitedArticles() {
        for( NewsArticle article : getArticlesPerView() ){
            addVisitedArticle(article);
        }
    }

    public synchronized static void addVisitedArticle( NewsArticle article ){
        visitedArticles.add( article.getTitle() );
        visitedList.add( article );
        article.setVisited( true );
    }

    public synchronized static void resetVisitedArt(){
        if( visitedArticles != null ) {
            visitedArticles.clear();
        }
    }

    public synchronized static String getArticlesWithFeedback(List<NewsArticle> list ) {
        if( list != null ) {
            NewsArticleVector vector = new NewsArticleVector();
            for (NewsArticle article : list) {
                if ( visitedArticles.contains(article.getTitle()) ) {
                    vector.add(mappings.get( article.getTitle() ));
                }
            }
            return vector.toJson();
        }else{
            return null;
        }
    }

    public synchronized static void increaseEndPosBatch( int endPos ){
        startPosBatch = endPosBatch;
        endPosBatch += endPos;
    }

    /**
     * It determines the current position of the cursor in NewsArticleVector. Also, it determines
     * whether to reload, refresh, send a feedback or request a ranked list of articles
     * @param goForward
     * @param refresh
     */
    public static void processCurrentArticle( boolean goForward, boolean refresh ) {
        ReaderController.getListView().setDwellTime(currentPos);
        if( goForward ){
            increaseCurrentPosition();
        }else{
            decreaseCurrentPosition();
        }

        if( refresh ) {
            setVisitedArticles();

            // if all the set of news articles (e.g. 170 articles) have been shown the user
            // then reload the list from Yahoo Slingstone
            if (currentPos == instance.size()) {
                ReaderController.setmLastReload(null);
                RequestFetchNewsEvent event = new RequestFetchNewsEvent(-1);
                event.setInitialize(true);
                EventBus.getDefault().post(event);
            } else {
                // if all the articles of the current batch have been already shown to the user
                // then reload the next batch of news articles
                if (currentPos == endPosBatch) {
                    ReaderController.getInstance().sendFeedbackTask();
                    ReaderController.getInstance().requestPersonalizationTask();
                }
                //otherwise, show the next article of the batch
                else {
                    ReaderController.sendRefreshNewsEvent();
                }
            }
        }
    }

    public static void release() {
        if( list1 != null ) {
            list1.clear();
            list1 = null;
        }
        if( list2 != null ) {
            list2.clear();
            list2 = null;
        }
        if( visitedArticles != null ) {
            visitedArticles.clear();
            visitedArticles = null;
        }
        if( visitedList != null ) {
            visitedList.clear();
            visitedList = null;
        }
    }

    /**
     * This method removes from instance the articles contained in @tempList. Then, it adds the articles
     * in @tempList at endPosBatch
     * @param tempList
     */
    public static void sortNews(List<NewsArticle> tempList) {
        List<NewsArticle> tempList2 = new ArrayList<>();
        for( NewsArticle article : tempList ){
            NewsArticle art = mappings.get(article.getTitle());
            tempList2.add(art);
            instance.remove(art);
        }
        instance.addAll(endPosBatch, tempList2);
        int count = 0;
        for( NewsArticle article : NewsArticleVector.getInstance() ) {
            article.setIdx(count++);
        }
    }

    public static void storeNewsOnPhone( ArrayList news ) {
        // Storing the news on external storage
        if( news != null ) {
            if (Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED)) {
                try {
                    File file = new File(Environment.getExternalStoragePublicDirectory(
                            Environment.DIRECTORY_DOWNLOADS), "news.json");
                    PrintWriter writer = new PrintWriter(file, "UTF-8");
                    writer.print(gson.toJson(news));
                    writer.flush();
                    writer.close();
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }
    }

    public static ArrayList getStoredList() {
        try {
            File file = new File(Environment.getExternalStoragePublicDirectory( Environment.DIRECTORY_DOWNLOADS), "news.json");
            String text = new Scanner( file, "UTF-8" ).useDelimiter("\\A").next();
            return fromJson( text, false );
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }
        return null;

    }

    public static ArrayList<NewsArticle> getVisitedList() {
        return visitedList;
    }
}
