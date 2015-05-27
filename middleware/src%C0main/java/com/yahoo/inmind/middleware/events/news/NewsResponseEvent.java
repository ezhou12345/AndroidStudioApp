package com.yahoo.inmind.middleware.events.news;

import com.yahoo.inmind.control.events.BaseEvent;
import com.yahoo.inmind.control.news.NewsArticleVector;

/**
 * Created by oscarr on 12/22/14.
 * This event retrieves the most recent list of news articles
 */
public class NewsResponseEvent extends BaseEvent {

    private Integer qualifier = 0;
    private NewsArticleVector news;
    private String jsonRepresentation;


    public NewsResponseEvent(Integer qualifier, NewsArticleVector news) {
        if( qualifier != null ) {
            this.qualifier = qualifier;
        }
        this.news = news;
    }

    public NewsResponseEvent(Integer qualifier, String jsonRrepresentation) {
        if( qualifier != null ) {
            this.qualifier = qualifier;
        }
        this.jsonRepresentation = jsonRrepresentation;
    }

    public NewsResponseEvent(){}

    public Integer getQualifier() {
        return qualifier;
    }

    public void setQualifier(Integer qualifier) {
        this.qualifier = qualifier;
    }

    public NewsArticleVector getNews() {
        return news;
    }

    public void setNews(NewsArticleVector news) {
        this.news = news;
    }

    public String getJsonRepresentation() {
        return jsonRepresentation;
    }

    public void setJsonRepresentation(String jsonRepresentation) {
        this.jsonRepresentation = jsonRepresentation;
    }
}
