package com.yahoo.inmind.control.util;

import android.content.Context;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.AsyncHttpRequest;
import com.loopj.android.http.RequestHandle;
import com.loopj.android.http.ResponseHandlerInterface;

import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.protocol.HttpContext;

import java.util.List;

/**
 * Created by oscarr on 2/25/15.
 */
public interface HttpCommInterface {
    public List<RequestHandle> getRequestHandles();
    public void addRequestHandle(RequestHandle handle);
    public Header[] getRequestHeaders( String stringHeaders );
    public HttpEntity getRequestEntity( String bodyText );
    public AsyncHttpClient getAsyncHttpClient();
    public void setAsyncHttpClient(AsyncHttpClient client);
    public AsyncHttpRequest getHttpRequest(DefaultHttpClient client, HttpContext httpContext, HttpUriRequest uriRequest, String contentType, ResponseHandlerInterface responseHandler, Context context);
    public ResponseHandlerInterface getResponseHandler();
    public String getDefaultURL();
    public RequestHandle execute(AsyncHttpClient client, String URL, Header[] headers, HttpEntity entity, ResponseHandlerInterface responseHandler);
}
