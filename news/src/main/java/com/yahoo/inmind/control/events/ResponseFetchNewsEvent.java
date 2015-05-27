package com.yahoo.inmind.control.events;

/**
 * Created by oscarr on 12/23/14.
 */
public class ResponseFetchNewsEvent extends BaseEvent {
    public ResponseFetchNewsEvent(Integer mbRequestId) {
        super( mbRequestId );
    }

}
