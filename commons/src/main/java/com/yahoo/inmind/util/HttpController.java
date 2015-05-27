package com.yahoo.inmind.util;

import android.content.Context;
import android.util.Log;

import com.android.volley.AuthFailureError;
import com.android.volley.DefaultRetryPolicy;
import com.android.volley.NetworkResponse;
import com.android.volley.RequestQueue;
import com.android.volley.VolleyError;
import com.android.volley.VolleyLog;
import com.android.volley.toolbox.HttpHeaderParser;
import com.android.volley.toolbox.Volley;
import com.goebl.david.Webb;
import com.yahoo.inmind.events.TestEvent;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.ResponseHandler;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.impl.client.BasicResponseHandler;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;

import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.util.Collections;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import de.greenrobot.event.EventBus;

/**
 * Created by oscarr on 2/25/15.
 */
public class HttpController {

    private static Webb webb;
    private static RequestQueue mRequestQueue;
    private static HttpClient httpclient = new DefaultHttpClient();
    private static HttpPost httppost;
    private static MultipartEntityBuilder entity = MultipartEntityBuilder.create();
    public static long time = 0;


//    public static String getHtpPostResponse( String url, Map<String, String> payload ){
//        HttpClient httpclient = new DefaultHttpClient();
//        HttpResponse httpResponse;
//        String response = null;
//        HttpPost httppost = new HttpPost( url );
//        try {
////            List<NameValuePair> nameValuePairs = new ArrayList<>();
////            for( String key : payload.keySet() ){
////                nameValuePairs.add(new BasicNameValuePair( key, payload.get(key) ));
////            }
////            httppost.setEntity( new UrlEncodedFormEntity(nameValuePairs) );
//
//
//            String content = "";
//            Iterator<String> it = payload.keySet().iterator();
//            while( it.hasNext()  ){
//                String key = it.next();
//                content += URLEncoder.encode( payload.get(key), "UTF-8");
//                if( it.hasNext() ){
//                    content += "&";
//                }
//            }
//            StringEntity se = new StringEntity( content );
//            se.setContentType("application/json;charset=UTF-8");
//            se.setContentEncoding(new BasicHeader(HTTP.CONTENT_TYPE,"application/json;charset=UTF-8"));
//
//
//            httppost.addHeader("content-type", "application/json");
//            httppost.addHeader("Accept","application/json");
//            httppost.setEntity(se);
//
//            //Log.e("", "content: " + content);
//
//            httpResponse = httpclient.execute( httppost );
//
//            InputStream in = httpResponse.getEntity().getContent();
//            StringBuilder stringbuilder = new StringBuilder();
//            BufferedReader bfrd = new BufferedReader(new InputStreamReader(in), 1024);
//            while( (response = bfrd.readLine()) != null )
//                stringbuilder.append( response );
//
//            response = stringbuilder.toString();
//        } catch (Exception e) {
//            e.printStackTrace();
//        } finally{
//            httpclient.getConnectionManager().shutdown();
//        }
//        return response;
//    }



    //Using goebl DavidWebb
    public static String getHttpPostResponse( String url, Map<String, Object> params, Object body, int timeout ){
        com.goebl.david.Response<String> response;
        String result = "";
        if( webb == null ) {
            webb = Webb.create();
        }

        try {
            com.goebl.david.Request request = webb.post( url );
            if( body == null ){
                Iterator<String> it = params.keySet().iterator();
                while (it.hasNext()) {
                    String key = it.next();
                    request.param( key, params.get(key ) );
                }
            } else{
                request.body( body );
            }

            response = request
                    //.compress() //check this
                    .connectTimeout( 10000 ) //timeout )
                    .asString();

            if (response.isSuccess()) {
                result = response.getBody();
            } else {
                Log.e("Util.HttpController", ""+response.getStatusCode() );
                Log.e("Util.HttpController", response.getResponseMessage());
                Log.e("Util.HttpController", response.getErrorBody().toString());
            }
        }catch (Exception e) {
            Log.d("InputStream", e.getLocalizedMessage());
            e.printStackTrace();
        }
        return result;
    }




    /** ========================================================================================== **/

    public static void uploadImage( Context context, String url, File imageFile, String name, int timeout ){
        if( mRequestQueue == null ){
            mRequestQueue = Volley.newRequestQueue( context );
        }

        MultipartRequest request = new MultipartRequest<>( url
                ,new com.android.volley.Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // Display the first 500 characters of the response string.
                        //Log.e("DEBUG", "Response is: "+ response.substring(0,500));
                    }
                }
                ,new com.android.volley.Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Log.e("DEBUG", "That didn't work!");
                    }
                }
                ,imageFile
                ,name);

        request.setRetryPolicy(new DefaultRetryPolicy(
                timeout,
                2,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));

        mRequestQueue.add( request );
    }



    static class MultipartRequest<T> extends com.android.volley.Request<T> {
        private static final String FILE_PART_NAME = "image";
        private MultipartEntityBuilder mBuilder = MultipartEntityBuilder.create();
        private final com.android.volley.Response.Listener<T> mListener;
        private final File mImageFile;
        protected Map<String, String> headers;
        private String mName;

        public MultipartRequest(String url,
                                     com.android.volley.Response.Listener<T> listener,
                                     com.android.volley.Response.ErrorListener errorListener,
                                     File imageFile,
                                     String name){
            super(Method.POST, url, errorListener);
            mListener = listener;
            mImageFile = imageFile;
            mName = name;
            buildMultipartEntity();
        }

        @Override
        public Map<String, String> getHeaders() throws AuthFailureError {
            Map<String, String> headers = super.getHeaders();
            if (headers == null
                    || headers.equals(Collections.emptyMap())) {
                headers = new HashMap<>();
            }
            //headers.put("Accept", "application/json");
            return headers;
        }

        private void buildMultipartEntity(){
            try {
                mBuilder.addBinaryBody(FILE_PART_NAME, mImageFile, ContentType.create("image/jpeg"), mName);
                //mBuilder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);
                //mBuilder.setLaxMode().setBoundary("xx").setCharset(Charset.forName("UTF-8"));
                //EventBus.getDefault().post(new TestEvent(mImageFile.getPath()));
            }catch (Exception e){

            }
        }

        @Override
        public String getBodyContentType(){
            String contentTypeHeader = mBuilder.build().getContentType().getValue();
            return contentTypeHeader;
        }

//        @Override
//        public byte[] getBody() throws AuthFailureError{
//            ByteArrayOutputStream bos = new ByteArrayOutputStream();
//            try{
//                mBuilder.build().writeTo(bos);
//            }catch (IOException e){
//                VolleyLog.e("IOException writing to ByteArrayOutputStream bos, building the multipart request.");
//            }
//            return bos.toByteArray();
//        }

        @Override
        protected com.android.volley.Response<T> parseNetworkResponse(NetworkResponse response){
            T result = null;
            return com.android.volley.Response.success(result, HttpHeaderParser.parseCacheHeaders(response));
        }

        @Override
        protected void deliverResponse(T response){
            mListener.onResponse(response);
        }
    }



    //@SuppressWarnings("deprecation")
    public static String uploadFile3( File sourceFile, String urlString ) {
        String responseString = "";

        try {
            MultipartEntityBuilder entity = MultipartEntityBuilder.create();
            String name = "monkey" + System.currentTimeMillis() + ".jpg";
            entity.addBinaryBody("image", sourceFile, ContentType.create("image/jpeg"), name);

            String boundary = "";
            URL url = new URL(urlString);
            HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setUseCaches(false);
            urlConnection.setDoOutput(true);
            urlConnection.setDoInput(true);
            urlConnection.setRequestMethod("POST");
            urlConnection.setRequestProperty("Accept-Charset", "UTF-8");
            urlConnection.setRequestProperty("Connection", "Keep-Alive");
            urlConnection.setRequestProperty("Cache-Control", "no-cache");
            urlConnection.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + boundary);
            urlConnection.setRequestProperty(entity.build().getContentType().getName(),
                    entity.build().getContentType().getValue());

            OutputStream os = urlConnection.getOutputStream();
            entity.build().writeTo(os);
            os.flush();
            os.close();


            BufferedReader in = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
            Log.e("", "response: " + in.readLine() );
            urlConnection.disconnect();




//            HttpClient httpclient = new DefaultHttpClient();
//            HttpPost httppost = new HttpPost(url);
//
//            MultipartEntityBuilder entity = MultipartEntityBuilder.create();
//            String name = "monkey" + System.currentTimeMillis() + ".jpg";
//            entity.addBinaryBody("image", sourceFile, ContentType.create("image/jpeg"), name);
//            httppost.setEntity(entity.build());
//
//            // Making server call
//            HttpResponse response = httpclient.execute(httppost);
//            HttpEntity r_entity = response.getEntity();
//
//            int statusCode = response.getStatusLine().getStatusCode();
//            if (statusCode == 200) {
//                // Server response
//                responseString = EntityUtils.toString(r_entity);
//            } else {
//                responseString = "Error occurred! Http Status Code: " + statusCode;
//            }
        }catch (Exception e){
            e.printStackTrace();
        }

        return responseString;

    }



    @SuppressWarnings("deprecation")
    public static String uploadImage( byte[] sourceFile, String url, String imageName ) {
        String responseString = "";

        try {
            if( httppost == null){
                httppost = new HttpPost(url);
            }

            if( entity == null ) {
                entity = MultipartEntityBuilder.create();
            }
            Log.e("","5. time: '" + (System.currentTimeMillis() - time ));
            time = System.currentTimeMillis();

            entity.addBinaryBody("image", sourceFile, ContentType.create("image/jpeg"), imageName);
            httppost.setEntity( entity.build() );
            Log.e("","6. time: '" + (System.currentTimeMillis() - time ));
            time = System.currentTimeMillis();

            // Making server call
            HttpResponse response = httpclient.execute( httppost );
            response.getEntity().consumeContent();


//            Log.e("","7. time: '" + (System.currentTimeMillis() - time ));
//            time = System.currentTimeMillis();
//
//            //response
//            HttpEntity r_entity = response.getEntity();
//            int statusCode = response.getStatusLine().getStatusCode();
//            if (statusCode == 200) {
//                // Server response
//                responseString = EntityUtils.toString(r_entity);
//            } else {
//                responseString = "Error occurred! Http Status Code: " + statusCode;
//            }
//            Log.e("","8. time: '" + (System.currentTimeMillis() - time ));
//            time = System.currentTimeMillis();
        }catch (Exception e){
            e.printStackTrace();
        }

        return responseString;

    }





    /** ========================================================================================== **/


    public static String getHttpGetResponse( String url, Map<String, String> payload ){
        com.goebl.david.Response<String> response;
        String result = "", content = "";
        if( webb == null ) {
            webb = Webb.create();
        }

        try {
            Iterator<String> it = payload.keySet().iterator();
            while( it.hasNext()  ){
                String key = it.next();
                content += key + "=" + URLEncoder.encode( payload.get(key), "UTF-8");
                if( it.hasNext() ){
                    content += "&";
                }
            }
            response = webb
                    .get( url+"?"+ content )
                    .connectTimeout(10 * 1000)
                    .asString();

            if (response.isSuccess()) {
                result = response.getBody();
            } else {
                Log.e("Util.HttpController", ""+response.getStatusCode() );
                Log.e("Util.HttpController", response.getResponseMessage());
                Log.e("Util.HttpController", response.getErrorBody().toString());
            }


        }catch (Exception e) {
            Log.d("InputStream", e.getLocalizedMessage());
            e.printStackTrace();
        }
        return result;
    }



    public static String getHttpGetResponseOld( String url, Map<String, String> payload ){
        InputStream inputStream;
        String result = "";
        try {
            // create HttpClient
            HttpClient httpclient = new DefaultHttpClient();

            String content = "";
            Iterator<String> it = payload.keySet().iterator();
            while( it.hasNext() ){
                String key = it.next();
                content += key + "=" + URLEncoder.encode( payload.get(key), "UTF-8");
                if( it.hasNext() ){
                    content += "&";
                }
            }

            // make GET request to the given URL
            HttpResponse httpResponse = httpclient.execute( new HttpGet( url+"?"+ content) );

            // receive response as inputStream
            inputStream = httpResponse.getEntity().getContent();

            // convert inputstream to string
            if(inputStream != null)
                result = convertInputStreamToString(inputStream);
            else
                result = "Did not work!";

        } catch (Exception e) {
            Log.d("InputStream", e.getLocalizedMessage());
            e.printStackTrace();
        }

        return result;
    }



// convert inputstream to String
    private static String convertInputStreamToString(InputStream inputStream) throws IOException {
        BufferedReader bufferedReader = new BufferedReader( new InputStreamReader(inputStream));
        String line;
        String result = "";
        while((line = bufferedReader.readLine()) != null)
            result += line;

        inputStream.close();
        return result;
    }



    public static void uploadFile( File sourceFile, String upLoadServerUri  ){
        HttpURLConnection conn = null;
        DataOutputStream dos = null;
        String lineEnd = "\r\n";
        String twoHyphens = "--";
        String boundary = "*****";
        int bytesRead, bytesAvailable, bufferSize;
        byte[] buffer;
        int maxBufferSize = 1 * 1024 * 1024;
        String fileName = sourceFile.getName();

        try {

            // open a URL connection to the Servlet
            FileInputStream fileInputStream = new FileInputStream(sourceFile);
            URL url = new URL( upLoadServerUri);

            // Open a HTTP  connection to  the URL
            conn = (HttpURLConnection) url.openConnection();
            conn.setDoInput(true); // Allow Inputs
            conn.setDoOutput(true); // Allow Outputs
            conn.setUseCaches(false); // Don't use a Cached Copy
            conn.setRequestMethod("POST");
            conn.setRequestProperty("Connection", "Keep-Alive");
            conn.setRequestProperty("ENCTYPE", "multipart/form-data");
            conn.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + boundary);
            conn.setRequestProperty("uploaded_file", fileName);

            dos = new DataOutputStream(conn.getOutputStream());

            dos.writeBytes(twoHyphens + boundary + lineEnd);
            String header = "Content-Disposition: form-data; name=\"uploaded_file\";filename="
                    + fileName + lineEnd;
            dos.writeBytes( header );
            dos.writeBytes(lineEnd);

            // create a buffer of  maximum size
            bytesAvailable = fileInputStream.available();

            bufferSize = Math.min(bytesAvailable, maxBufferSize);
            buffer = new byte[bufferSize];

            // read file and write it into form...
            bytesRead = fileInputStream.read(buffer, 0, bufferSize);

            while (bytesRead > 0) {

                dos.write(buffer, 0, bufferSize);
                bytesAvailable = fileInputStream.available();
                bufferSize = Math.min(bytesAvailable, maxBufferSize);
                bytesRead = fileInputStream.read(buffer, 0, bufferSize);

            }

            // send multipart form data necesssary after file data...
            dos.writeBytes(lineEnd);
            dos.writeBytes(twoHyphens + boundary + twoHyphens + lineEnd);

            // Responses from the server (code and message)
            int serverResponseCode = conn.getResponseCode();
            String serverResponseMessage = conn.getResponseMessage();

            Log.i("uploadFile", "HTTP Response is : "
                    + serverResponseMessage + ": " + serverResponseCode);


            //close the streams //
            fileInputStream.close();
            dos.flush();
            dos.close();

        } catch (MalformedURLException ex) {

            Log.e("Upload file to server", "error: " + ex.getMessage(), ex);
        } catch (Exception e) {

        }
    }





    public static void uploadFile2( File file, String url ){
        HttpClient httpclient = new DefaultHttpClient();
        HttpPost httppost = new HttpPost( url );

        try {
            FileBody bin = new FileBody( file );
            StringBody comment = new StringBody("BETHECODER HttpClient Tutorials");

            MultipartEntityBuilder reqEntity = MultipartEntityBuilder.create();
            reqEntity.addPart("fileup0", bin);
//            reqEntity.addPart("fileup1", comment);
//
//            reqEntity.addPart("ONE", new StringBody("11111111"));
//            reqEntity.addPart("TWO", new StringBody("222222222"));
            httppost.setEntity( reqEntity.build() );

            System.out.println("Requesting : " + httppost.getRequestLine());
            ResponseHandler<String> responseHandler = new BasicResponseHandler();
            String responseBody = httpclient.execute(httppost, responseHandler);

            System.out.println("responseBody : " + responseBody);

        } catch (Exception e) {
            e.printStackTrace();
        }  finally {
            httpclient.getConnectionManager().shutdown();
        }
    }



}


