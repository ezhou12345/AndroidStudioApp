package com.yahoo.inmind.middleware.control;

import android.os.Bundle;

/**
 * Created by oscarr on 12/5/14.
 */
public interface NewsObservable {

    public void subscribe( NewsObserver o, String topic );

    public void unSubscribe( NewsObserver o, String topic );

    public void sendRequest( int requestId, Bundle obj);

    public void notifyObserversPublish(String topic);

    public void notifyObserversQueue(String topic);
}
