package com.yahoo.inmind.control.events;

/**
 * Created by oscarr on 12/23/14.
 */
public class RequestFetchNewsEvent extends BaseEvent{
    private Integer mbRequestId;
    private Integer articleId;
    private boolean initialize;

    public RequestFetchNewsEvent(Integer mbRequestId) {
        this.mbRequestId = mbRequestId;
    }

    public Integer getMbRequestId() {
        return mbRequestId;
    }

    public void setMbRequestId(Integer mbRequestId) {
        this.mbRequestId = mbRequestId;
    }

    public Integer getArticleId() {
        return articleId;
    }

    public void setArticleId(Integer articleId) {
        this.articleId = articleId;
    }

    public void setInitialize(boolean initialize) {
        this.initialize = initialize;
    }

    public boolean isInitialize() {
        return initialize;
    }
}
