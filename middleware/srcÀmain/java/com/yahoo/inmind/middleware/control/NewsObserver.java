package com.yahoo.inmind.middleware.control;

import com.yahoo.inmind.model.vo.NewsArticle;

import java.util.List;

/**
 * Created by oscarr on 12/5/14.
 */
public interface NewsObserver {

    /**
     * This method notifies all the news subscribers
     * when the list of news articles changes
     * @param news
     */
    public void updateNews( List<NewsArticle> news );



}
