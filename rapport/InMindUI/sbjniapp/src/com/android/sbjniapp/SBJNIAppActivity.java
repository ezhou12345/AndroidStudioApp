package com.android.sbjniapp;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.util.Log;
import android.view.WindowManager;

import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;
import android.view.LayoutInflater;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.Menu;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;



import java.io.File;


public class SBJNIAppActivity extends Activity {
	
//	protected boolean wantsMultitouch = false;
//    protected float multiData[];
//    protected static final int MULTI_DATA_STRIDE = 5;
//    protected static final int MULTI_MAX_INPUTS = 10;
//    protected SBJNIAppView sbView;
//    
//    
//    public static native String openConnection();
//   
//   
//    static {
//   	 //System.loadLibrary("python2.6");
//   	 //System.loadLibrary("sbm");
//        System.loadLibrary("sbjniapp");
//    }

    
    @Override protected void onCreate(Bundle icicle) {
        super.onCreate(icicle);        
        setContentView(R.layout.main);
        
//        Button button = (Button)findViewById(R.id.SbmButton);  
        System.out.println("#############$$$$$$$$$$$$$$$$");
        
        VhmsgWrapper vhmsg=new VhmsgWrapper();
        vhmsg.openConnection();
        String vrSpeak="Brad User 1303332588320-128-1\n"
				+ "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<act>\n"
				+ "<participant id=\"Brad\" role=\"actor\" />\n<bml>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"RIGHT\" angle=\"30\" start=\"0\" ready=\"3.5\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"LEFT\" angle=\"10\" start=\"3.5\" ready=\"4\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"LEFT\" angle=\"0\" start=\"4\" ready=\"5\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<head id=\"behavior1\" type = \"NOD\" repetition= \"1\" amount = \"0.3\" start= \"3\"/>\n"
				+ "<animation id = \"animation1\" start=\"sp1:T0\" priority=\"5\" name=\"ChrBrad@Entrance\" />"
				+ "<event message=\"vrSpoke Rachel User 1404332904389-28-1 entrance animation end.\" stroke=\"animation1:end\" xmlns:sbm=\"http://ict.usc.edu\"/>"
				+ "\n</bml>\n</act>";
        vhmsg.send("vrSpeak", vrSpeak);
        vhmsg.closeConnection();
        System.out.println("#############$$$$$$$$$$$$$$$$");
     //   sbView = (SBJNIAppView)findViewById(R.id.surfaceView1);    	
        //sbView.setPreserveEGLContextOnPause(true);
    }

//    @Override protected void onPause() {
//        super.onPause();
//      
//        sbView.onPause();
//        //mView.onPause();
//    }
//    
//    @Override
//    public boolean onTouchEvent(MotionEvent event)
//    {
//        boolean ret = super.onTouchEvent(event);
//        if (!ret)
//        {
//        	if (wantsMultitouch)
//        	{
//	        	// marshal up the data.
//	        	int numEvents = event.getPointerCount();
//	        	for (int i=0; i<numEvents; i++)
//	        	{
//	        		int j = i*MULTI_DATA_STRIDE;
//	        		// put x and y FIRST, so if people just want that data, there's nothing else
//	        		// to jump over...
//	        		multiData[j + 0] = (float)event.getX(i);
//	        		multiData[j + 1] = (float)event.getY(i);
//	        		multiData[j + 2] = (float)event.getPointerId(i);
//	        		multiData[j + 3] = (float)event.getSize(i);
//	        		multiData[j + 4] = (float)event.getPressure(i);
//	        	}
//	            ret = multitouchEvent(event.getAction(), numEvents, multiData, MULTI_DATA_STRIDE, event);
//        	}
//        	else // old style input.
//        	{
//              //  ret = inputEvent(event.getAction(), event.getX(), event.getY(), event);
//        	}
//        }
//        return ret;
//    }
    
    public boolean multitouchEvent(int action, int numInputs, float data[], int dataStride, MotionEvent event)
    {
    	
    	return true;
    }
    
    

//    @Override protected void onResume() {
//    	/*
//    	try {
//    		Thread.sleep(1000*15);
//    		
//    	}
//    	catch (Exception e)
//    	{
//    	
//    	}
//    	*/
//        super.onResume();
//       // SBJNIAppLib.executeSB("sim.resume()"); 
//        sbView.onResume();
//        //mView.onResume();
//    }
//    
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
    	MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.menu, menu);
        return super.onCreateOptionsMenu(menu);
    }
  //  @Override
  //  public boolean onOptionsItemSelected(MenuItem item) {
        // Handle all of the possible menu actions.
//        switch (item.getItemId()) {
//        case R.id.StartSim:    
//        	onResume();
//            break;
//        case R.id.PauseSim:
//        	onPause();
//            break;
//        case R.id.RestartSim:   
//        	sbView.restartSB();
//        	break;
//        case R.id.ShowLog:   
//        	showDialog(0);
//        	break;
//        case R.id.Connect:
//        	SBJNIAppLib.openConnection();        	
//            break;
//        case R.id.Disconnect:
//        	SBJNIAppLib.closeConnection();        	
//            break;
//        }
//        return super.onOptionsItemSelected(item);
//        
//    }
//    @Override
//    protected Dialog onCreateDialog(int id) {
//        Dialog dialog;        
//        return createLogDialog();
//    }
//    
//    @Override
//    protected void onPrepareDialog(int id, Dialog dialog) {     
//    	AlertDialog alertDialog = (AlertDialog)dialog;
//    	TextView text = (TextView)dialog.findViewById(R.id.TextView02); 
//    	String logs = SBJNIAppLib.getLog();
//    	text.setText(logs);
//  	  	//text.setText("fjajfiosajgihagosdghaogiha");    	
//    }    
//    public AlertDialog createLogDialog(){    	
//    	  AlertDialog.Builder ad = new AlertDialog.Builder(this);
//    	  ad.setIcon(R.drawable.icon);
//    	  ad.setTitle("Smartbody Log");   	    
//    	  ad.setView(LayoutInflater.from(this).inflate(R.layout.custom_dialog,null));    	  
//    	  ad.setPositiveButton("OK", new android.content.DialogInterface.OnClickListener() {
//    		public void onClick(DialogInterface dialog, int which) {
//				// TODO Auto-generated method stub
//				
//			}  }  );	    	  
//    	  
//    	  AlertDialog dialog = ad.create();    	  
//    	  return dialog;    	  
//     }
//    
//    private OnClickListener sbmListenser = new OnClickListener()
//    {
//        public void onClick(View v)
//        {
//        	EditText sbmTextEdit = (EditText)findViewById(R.id.SbmText);
//        	String sbmCmd = sbmTextEdit.getText().toString();
//        	/*
//        	CheckBox sbmPyCheck = (CheckBox)findViewById(R.id.usePython);        	
//        	if (sbmPyCheck.isChecked())
//        	{
//        		SbmJNILib.executePython(sbmCmd);        		
//        	}
//        	else
//        	*/
//        	//SbmJNILib.executeSbm(sbmCmd); 
//        	SBJNIAppLib.executeSB(sbmCmd);  
//        	sbmTextEdit.getText().clear();
//        }
//    };
}