package com.yahoo.inmind.control.events;

/**
 * Created by oscarr on 4/2/15.
 */
public class TestEvent {
    String path;

    public String getPath() {
        return path;
    }

    public void setPath(String path) {
        this.path = path;
    }


    public TestEvent(String path) {
        this.path = path;
    }
}
