package com.yahoo.inmind.your_app.control;

import android.content.Context;

import com.yahoo.inmind.middleware.control.MessageBroker;
import com.yahoo.inmind.your_app.view.ViewHelper;

/**
 * Created by oscarr on 12/3/14.
 */
public class SingletonApp {

    private static SingletonApp instance;
    public static MessageBroker mMB;
    public static Context context;
    // add your global objects here...

    private SingletonApp( Context ctx ) {
        mMB = MessageBroker.getInstance( ctx );
        context = ctx;
    }


    public static SingletonApp getInstance(Context context) {
        if (instance == null) {
            instance = new SingletonApp( context );
        }
        return instance;
    }


    public static Context getContext() {
        return context;
    }
}
