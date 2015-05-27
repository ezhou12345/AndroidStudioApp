package com.yahoo.inmind.middleware.events.news;

import com.yahoo.inmind.control.events.BaseEvent;
import com.yahoo.inmind.control.news.NewsArticleVector;

/**
 * Created by oscarr on 12/22/14.
 * This event notify the subscriber when the list of news articles has changed.
 */
public class NewsUpdateEvent extends BaseEvent {
    private NewsArticleVector news;

    public NewsUpdateEvent(NewsArticleVector news) {
        this.news = news;
    }

    public NewsArticleVector getNews() {
        return news;
    }

    public void setNews(NewsArticleVector news) {
        this.news = news;
    }
}
