package com.yahoo.inmind.control.events;

import com.yahoo.inmind.control.news.NewsArticleVector;

/**
 * Created by oscarr on 3/10/15.
 */
public class RefreshNewsListEvent extends BaseEvent{
    private NewsArticleVector articleList;

    public NewsArticleVector getArticleList() {
        return articleList;
    }

    public void setArticleList(NewsArticleVector articleList) {
        this.articleList = articleList;
    }
}
