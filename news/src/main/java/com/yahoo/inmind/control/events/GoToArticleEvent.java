package com.yahoo.inmind.control.events;

/**
 * Created by oscarr on 2/3/15.
 */
public class GoToArticleEvent extends BaseEvent{
    private int idx;

    public int getIdx() {
        return idx;
    }

    public void setIdx(int idx) {
        this.idx = idx;
    }
}
