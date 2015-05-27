package com.android.sbjniapp;

public class VhmsgWrapper {
	  public static native void openConnection();  
	  public static native void closeConnection();
	  public static native void send(String opname, String message);
	  
	
	  
	    static {
	   	 //System.loadLibrary("python2.6");
	   	 //System.loadLibrary("sbm");
	        System.loadLibrary("sbjniapp");
	    }

}
