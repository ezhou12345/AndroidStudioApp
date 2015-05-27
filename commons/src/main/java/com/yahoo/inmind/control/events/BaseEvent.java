package com.yahoo.inmind.control.events;

/**
 * Created by oscarr on 1/22/15.
 */
public class BaseEvent {
    private Integer mbRequestId;
    private Object subscriber;

    public BaseEvent(){}

    public BaseEvent(Integer mbRequestId) {
        this.mbRequestId = mbRequestId;
    }

    public Integer getMbRequestId() {
        return mbRequestId;
    }

    public void setMbRequestId(Integer mbRequestId) {
        this.mbRequestId = mbRequestId;
    }


    public Object getSubscriber() {
        return subscriber;
    }

    public void setSubscriber(Object subscriber) {
        this.subscriber = subscriber;
    }
}
