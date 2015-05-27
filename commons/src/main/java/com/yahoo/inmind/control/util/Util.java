package com.yahoo.inmind.control.util;

import android.annotation.TargetApi;
import android.content.Context;
import android.content.res.AssetManager;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import com.rits.cloning.Cloner;


import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Field;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

/**
 * Created by oscarr on 12/8/14.
 */
public class Util {

    private static Gson gson = new GsonBuilder().create();
    private static Cloner cloner = new Cloner();

    public static Properties loadConfigAssets( Context app, String propName ) {
        Properties properties = new Properties();
        AssetManager am = app.getAssets();

        InputStream inputStream;
        try {
            inputStream = am.open( propName );
            if (inputStream != null) {
                properties.load(inputStream);
            } else {
                throw new FileNotFoundException("property file '" + propName + "' not found in the classpath");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return properties;
    }

    public static <T> List<T> fromJsonList( String jsonList, Class<T> element ){
        Type type = new TypeToken<List<T>>() {}.getType();
        return gson.fromJson(jsonList, type);
    }

//
//    public static <T> String toJsonList( List<T> list ){
//        Type listType = new TypeToken<List<T>>(){}.getType();
//        return gson.toJson( list, listType );
//    }

    //    public static JSONArray fromJsonList(String json){
//        JSONArray obj = null;
//        try {
//            obj = (JSONArray) parser.parse(json);
//        } catch (ParseException e) {
//            e.printStackTrace();
//        }
//        return obj;
//    }


    public static <T> String toJson( T object ){
        return gson.toJson( object );
    }

    public static <T> T fromJson( String json, Class<T> clazz ){
        return gson.fromJson( json, clazz );
    }

    public static <T> T clone( T object ){
        return cloner.deepClone( object );
    }

    public static <T extends ArrayList> T cloneList( T list ){
        return cloner.deepClone( list );
    }

    public static String toJsonList( List list ){
        StringBuilder sb = new StringBuilder("[");
        Field[] fields = null;
        boolean firstObject = true;
        for (Object obj : list){
            if (firstObject){
                sb.append("{");
                firstObject = false;
            }else{
                sb.append(", {");
            }
            if (fields == null){
                fields = obj.getClass().getFields();
            }
            //do sth to retrieve each field value -> json property of json object
            //add to json array
            for (int i = 0 ; i < fields.length ; i++){
                Field f = fields[i];
                //jsonFromField(sb, obj, i, f);
            }
            sb.append("}");
        }
        sb.append("]}");
        return sb.toString();
    }


    public static String replaceAll(String str, String pat, String rep){
        if (str == null)
            return null;
        return str.replaceAll(pat, rep);
    }


    @TargetApi(19)
    public static String listToString( List list ){
        StringBuilder builder = new StringBuilder();
        for( Object obj : list ){
            builder.append( obj.toString() + System.lineSeparator() );
        }
        return builder.toString();
    }


}
