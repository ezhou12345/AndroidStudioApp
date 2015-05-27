package com.yahoo.inmind.reader;

import java.util.LinkedList;
import java.util.Queue;

import android.R.bool;
import android.app.Fragment;
import android.app.FragmentManager;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Color;
import android.graphics.Point;
import android.graphics.PointF;
import android.os.Bundle;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.util.Log;
import android.view.Display;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnTouchListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.RelativeLayout;
import android.widget.FrameLayout.LayoutParams;
import android.widget.TextView;
import android.widget.Toast;

import com.android.sbjniapp.VhmsgWrapper;
import com.unity3d.player.UnityPlayer;
import com.yahoo.inmind.browser.LoginBrowser;
import com.yahoo.inmind.handler.UIHandler;
import com.yahoo.inmind.i13n.I13N;
import com.yahoo.inmind.i13n.I13NActivity;
import com.yahoo.inmind.util.MemUtil;


public class ReaderMainActivity extends I13NActivity  {
		
	private static UnityPlayer mUnityPlayer;
	//private boolean news_mode_on = false;
    private static final int MAX_MEMORY = 160;
	private static final int MAX_CACHED_FRAGMENTS = 0;
	private NewsListFragment mCurrentFrag = null;
    private DrawerManager mDm = null;
    UIHandler mUiHandler; 
    Queue<NewsListFragment> que = new LinkedList<NewsListFragment>();//Cached pages
	private int mIconResSwitchView = R.drawable.ic_flip;
	boolean bSwitchDisabled = true;
	public static VhmsgWrapper vhmsg = null;
	private boolean assistant_button_clicked = false;
	private boolean news_button_clicked = false;
	private boolean news_mode_clicked = false;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.news_main);
        
       
        if (savedInstanceState == null) {
            getFragmentManager().beginTransaction()
                    .add(R.id.test_layout, new PlaceholderFragment())
                    .commit();
        }
        
        Display display = getWindowManager().getDefaultDisplay();
        Point size = new Point();
        display.getSize(size);
        final int width = size.x;
        final int height = size.y;
        final int news_height = height/2;
        final int assistant_height = height - news_height - 400 - 5; // 144 is button height, add 5 to avoid cut off
        final int small_assistant_height = height/5;
        final int small_assistant_width = width/4;
        final int assistant_button_width = width*3/10 - 10;
        final int news_button_width = width/3 - 10;
        final int news_mode_button_width = width - assistant_button_width - news_button_width;
        
        
       // Integrate two layout together Created by Ran Zhao 
        final FrameLayout layoutFrame = new FrameLayout(this);
//        final RelativeLayout layoutMove = new RelativeLayout(this);
//        layoutMove.setId(2003);
        final LinearLayout layoutMain = new LinearLayout(this);
        layoutMain.setOrientation(LinearLayout.VERTICAL);
        final LinearLayout button_panel = new LinearLayout(this);
        button_panel.setOrientation(LinearLayout.HORIZONTAL);
        button_panel.setBackgroundColor(Color.parseColor("#000000"));
        //button_panel.setBackgroundResource(R.drawable.light_grey_border);;
        layoutMain.addView(button_panel, LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        layoutFrame.addView(layoutMain);
        layoutMain.setBackgroundColor(Color.parseColor("#000000"));
        
        
        setContentView(layoutFrame);
        final LayoutInflater inflate = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        
        final DrawerLayout layoutLeft = (DrawerLayout) inflate.inflate(
                R.layout.news_main, null);
        
        final Button assistant_button = new Button(this);
        //close_news_button.setText("Hide News");
        assistant_button.setTextSize(12);
        assistant_button.setId(2000);
//		close_news_button.setY(760);
        //close_news_button.setPadding(3, 3, 3, 3);
        assistant_button.setTextColor(Color.parseColor("#000000"));
        assistant_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
        assistant_button.setBackgroundResource(R.drawable.assistant_button);
		//close_button.setBackgroundColor(Color.parseColor("#FFFFFF"));
        //310
		LinearLayout.LayoutParams assistant_button_params = new LinearLayout.LayoutParams(assistant_button_width,
                150);
		assistant_button_params.setMargins(5, 5, 0, 5);
		assistant_button.setLayoutParams(assistant_button_params);
		
		button_panel.addView(assistant_button, assistant_button_params);
		
		final Button news_button = new Button(this);
		//close_assistant_button.setText("Hide Assistant");
		news_button.setTextSize(12);
		news_button.setId(2001);
//		close_button.setY(760);
		//close_assistant_button.setPadding(3, 3, 3, 3);
		news_button.setTextColor(Color.parseColor("#000000"));
		news_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
		news_button.setBackgroundResource(R.drawable.news_button);
		//close_button.setBackgroundColor(Color.parseColor("#FFFFFF"));
		//350
		LinearLayout.LayoutParams news_button_params = new LinearLayout.LayoutParams(news_button_width,
				150);
		news_button_params.setMargins(10, 5, 0, 5);
		news_button.setLayoutParams(news_button_params);
		button_panel.addView(news_button, news_button_params);
		
		final Button news_mode_button = new Button(this);
		//news_mode_button.setText("Enter News Mode");
		news_mode_button.setTextSize(12);
		news_mode_button.setId(2002);
//		close_button.setY(760);
		//close_assistant_button.setPadding(3, 3, 3, 3);
		news_mode_button.setTextColor(Color.parseColor("#000000"));
		news_mode_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
		news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
		//close_button.setBackgroundColor(Color.parseColor("#FFFFFF"));
		//420
		LinearLayout.LayoutParams news_mode_button_params = new LinearLayout.LayoutParams(
				news_mode_button_width,150);
		news_mode_button_params.setMargins(10, 5, 10, 5);
		news_mode_button.setLayoutParams(news_mode_button_params);
		button_panel.addView(news_mode_button, news_mode_button_params);
		
		//1000
		DrawerLayout.LayoutParams layout_left_params = new DrawerLayout.LayoutParams(
				width, news_height);
		//layoutLeft.setBackgroundResource(R.drawable.blue_border);
        layoutMain.addView(layoutLeft, layout_left_params);
        
        
        DrawerLayout.LayoutParams drwParam= new DrawerLayout.LayoutParams(
       
        		DrawerLayout.LayoutParams.WRAP_CONTENT,
        		DrawerLayout.LayoutParams.WRAP_CONTENT);
        
        
        mUnityPlayer = new UnityPlayer(this);
        int glesMode = mUnityPlayer.getSettings().getInt("gles_mode", 1);
        mUnityPlayer.init(glesMode, false);
        
        //mUnityPlayer.getView().setBackgroundResource(R.drawable.blue_border);
        
        final RelativeLayout layoutRight = (RelativeLayout) inflate.inflate(
                R.layout.fragment_main, null);
        //layoutRight.setId(R.id.layout_right);

        RelativeLayout.LayoutParams relParam = new RelativeLayout.LayoutParams(
            RelativeLayout.LayoutParams.WRAP_CONTENT,
            RelativeLayout.LayoutParams.WRAP_CONTENT);
        
      //  RelativeLayout.LayoutParams relParam = new RelativeLayout.LayoutParams(750,640);
           //650
            layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
        
            assistant_button.setOnClickListener(new OnClickListener(){
          	   public void onClick(View v) {
          		 assistant_button_clicked = true;
          		assistant_button.setBackgroundResource(R.drawable.assistant_button_pressed);
          		  // close_news_clicked = !close_news_clicked;
          		   //if (close_news_clicked) {
          			   if (news_button_clicked) {
          				 news_button_clicked = false;
          				   news_button.setBackgroundResource(R.drawable.news_button);
          				   layoutMain.removeView(layoutLeft);
	           			   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
	           			   //close_assistant_button.setText("Hide Assistant");
	           			   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
          			   }
          			   if (news_mode_clicked) {
          				   news_mode_clicked = false;
          				   news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
          				   //news_mode_on = false;
          				   layoutMain.removeView(layoutRight);
          				   layoutMain.removeView(layoutLeft);
          				   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
          				   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
          			   }
          			   
  	        		   //Toast.makeText(getApplicationContext(), Integer.toString(width),
  	            		//	   Toast.LENGTH_LONG).show();
          			 //Toast.makeText(getApplicationContext(), Integer.toString(close_news_button.getHeight()),
      			   //Toast.LENGTH_SHORT).show();
  	        		   layoutLeft.setVisibility(View.GONE);
  	        		   
  	        		   
//  	        		   news_mode_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//  	        		   news_mode_button.setTextColor(Color.parseColor("#FFFFFF"));
//  	        		   news_mode_button.setEnabled(false);
//  	        		   close_assistant_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//  	        		   close_assistant_button.setTextColor(Color.parseColor("#FFFFFF"));
//  	        		   news_mode_button.setBackgroundResource(R.drawable.news_mode_button_disabled);
//  	        		   close_assistant_button.setEnabled(false);
//  	        		   close_assistant_button.setBackgroundResource(R.drawable.news_button_disabled);
  	        		   
  	        		   
  	        		   //close_button.setY(1550);
  	        		   //close_news_button.setText("Show News");
//  	        		   RelativeLayout layoutRight = (RelativeLayout)findViewById(R.id.layout_right);
//  	        		   if (layoutMain.findViewById(R.id.layout_right) != null) {
  	        		   layoutMain.removeView(layoutRight);
  	        		   //1600
  	        		   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, height - 400);
//  	        		   }
  	        		   
//  	        		   layoutMain.removeView(layoutRight);
//          		   } else {
//          			   //Toast.makeText(getApplicationContext(), "Clicked Show!",
//  	            			   //Toast.LENGTH_LONG).show();
//          			   layoutMain.removeView(layoutRight);
//        			   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
//        			   //close_news_button.setText("Hide News");
//  	        		   //close_button.setY(10);
//  	        		   layoutLeft.setVisibility(View.VISIBLE);
//  	        		   news_mode_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//	        		   news_mode_button.setTextColor(Color.parseColor("#000000"));
//  	        		   news_mode_button.setEnabled(true);
//  	        		   news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
//  	        		   close_assistant_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//  	        		   close_assistant_button.setTextColor(Color.parseColor("#000000"));
//  	        		   close_assistant_button.setEnabled(true);
//  	        		   close_assistant_button.setBackgroundResource(R.drawable.news_button);
//  	        		  
//          		   }
          	   }
          	});
            
            news_button.setOnClickListener(new OnClickListener(){
           	   public void onClick(View v) {
           		   //close_assistant_clicked = !close_assistant_clicked;
           		   //if (close_assistant_clicked) {
           		   news_button_clicked = true;
           		news_button.setBackgroundResource(R.drawable.news_button_pressed);
           			   if (assistant_button_clicked) { 
           				   assistant_button_clicked = false;
           				   assistant_button.setBackgroundResource(R.drawable.assistant_button);
           				   layoutMain.removeView(layoutRight);
           				   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
           				   layoutLeft.setVisibility(View.VISIBLE);
           			   }
           			   if (news_mode_clicked) {
           				   news_mode_clicked = false;
           				   news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
           				  // news_mode_on = false;
           				   layoutMain.removeView(layoutRight);
           				   layoutMain.removeView(layoutLeft);
           				   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
           				   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
           			   }
   	        		   //Toast.makeText(getApplicationContext(), "Clicked Hide!",
   	            		//	   Toast.LENGTH_LONG).show();
   	        		   //layoutRight.setVisibility(View.GONE);
   	        		   layoutMain.removeView(layoutRight);
   	        		   //close_button.setY(1550);
   	        		   
//   	        		   news_mode_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//	        		   news_mode_button.setTextColor(Color.parseColor("#FFFFFF"));
//   	        		   news_mode_button.setEnabled(false);
//   	        		   news_mode_button.setBackgroundResource(R.drawable.news_mode_button_disabled);
//   	        		   close_news_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//   	        		   close_news_button.setTextColor(Color.parseColor("#FFFFFF"));
//   	        		   close_news_button.setEnabled(false);
//   	        		   close_news_button.setBackgroundResource(R.drawable.assistant_button_disabled);
   	        		   
   	        		   
   	        		   //close_assistant_button.setText("Show Assistant");
//   	        		   RelativeLayout layoutRight = (RelativeLayout)findViewById(R.id.layout_right);
//   	        		   if (layoutMain.findViewById(R.id.layout_right) != null) {
   	        		   layoutMain.removeView(layoutLeft);
   	        		   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, height );
//   	        		   }
   	        		   
//   	        		   layoutMain.removeView(layoutRight);
//           		   } else {
//           			   //Toast.makeText(getApplicationContext(), "Clicked Show!",
//   	            		//	   Toast.LENGTH_LONG).show();
//           			   layoutMain.removeView(layoutLeft);
//         			   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
//         			   //close_assistant_button.setText("Hide Assistant");
//         			   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
//         			   news_mode_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//	        		   news_mode_button.setTextColor(Color.parseColor("#000000"));
//         			   news_mode_button.setEnabled(true);
//         			   news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
//         			   close_news_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//         			   close_news_button.setTextColor(Color.parseColor("#000000"));
//         			   close_news_button.setEnabled(true);
//         			   close_news_button.setBackgroundResource(R.drawable.assistant_button);
//           		   }
           	   }
           	});
            
            
            news_mode_button.setOnClickListener(new OnClickListener(){
            	   public void onClick(View v) {
            		   //news_mode_clicked = !news_mode_clicked;
            		   //if (news_mode_clicked) {
            			   //news_mode_on = true;
            		   news_mode_clicked = true;
            		   news_mode_button.setBackgroundResource(R.drawable.news_mode_button_pressed);
            			   if (assistant_button_clicked) {
            				   assistant_button_clicked = false;
            				   assistant_button.setBackgroundResource(R.drawable.assistant_button);
               				   layoutMain.removeView(layoutRight);
               				   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
               				   layoutLeft.setVisibility(View.VISIBLE);
            			   } 
            			   if (news_button_clicked) {
            				   news_button_clicked = false;
            				   news_button.setBackgroundResource(R.drawable.news_button);
              				   layoutMain.removeView(layoutLeft);
    	           			   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
    	           			   layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
            			   }
    	        		   //Toast.makeText(getApplicationContext(), "Clicked news mode!",
    	            		//	   Toast.LENGTH_LONG).show();
    	        		   //layoutRight.setVisibility(View.GONE);
    	        		   layoutMain.removeView(layoutRight);
    	        		   
    	        		   
//    	        		   close_assistant_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//    	        		   close_assistant_button.setTextColor(Color.parseColor("#FFFFFF"));
//    	        		   close_assistant_button.setEnabled(false);
//    	        		   close_assistant_button.setBackgroundResource(R.drawable.news_button_disabled);
//    	        		   close_news_button.setBackgroundColor(Color.parseColor("#DEDEDE"));
//    	        		   close_news_button.setTextColor(Color.parseColor("#FFFFFF"));
//    	        		   close_news_button.setEnabled(false);
//    	        		   close_news_button.setBackgroundResource(R.drawable.assistant_button_disabled);
    	        	
    	        		   
    	        		   
    	        		   //LinearLayout.LayoutParams news_mode_params = (LinearLayout.LayoutParams)news_mode_button.getLayoutParams();
    	        		   //news_mode_params.setMargins(310, 0, 0, 0); //substitute parameters for left, top, right, bottom
    	        		   //news_mode_button.setLayoutParams(news_mode_params);
    	        		   
    	        		   //close_button.setY(1550);
    	        		   //news_mode_button.setText("Exit News Mode");
//    	        		   Relativnews_mode_buttontMain.findViewById(R.id.layout_right) != null) {
    	        		   layoutMain.removeView(layoutLeft);
    	        		   
    	        		   LinearLayout.LayoutParams small_assistant_params = new LinearLayout.LayoutParams(
    	        				   LayoutParams.MATCH_PARENT, small_assistant_height);
    	        		   //small_assistant_params.gravity = Gravity.CENTER_HORIZONTAL;
    	        		   layoutRight.setLayoutParams(small_assistant_params);
    	        		   layoutMain.addView(layoutRight, small_assistant_params);
    	        		   layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, height - small_assistant_height - 400);
    	        		   
    	        		   
    	        		   //layoutFrame.addView(layoutRight, small_assistant_params);
    	        		   
    	        		   
    	        		   //button_panel.setBackgroundResource(0);
    	        	       //button_panel.setBackgroundColor(Color.parseColor("#000000"));
    	        	       
//    	        		   }
    	        		   
//    	        		   layoutMain.removeView(layoutRight);
//            		   } else {
//            			   news_mode_on = false;
//            			  // Toast.makeText(getApplicationContext(), "Clicked leave news mode!",
//    	            		//	   Toast.LENGTH_LONG).show();
//            			   //moveable_layout.removeView(layoutRight);
//            			   layoutFrame.removeView(layoutRight);
//            			   //button_panel.removeView(news_mode_button);
//            			   //button_panel.addView(close_news_button);
//            			   //button_panel.addView(close_assistant_button);
//            			   //button_panel.addView(news_mode_button);
//            			   close_assistant_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//    	        	       close_assistant_button.setTextColor(Color.parseColor("#000000"));
//             			   close_assistant_button.setEnabled(true);
//             			   close_assistant_button.setBackgroundResource(R.drawable.news_button);
//             			   close_news_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
//             			   close_news_button.setTextColor(Color.parseColor("#000000"));
//  	        	           close_news_button.setEnabled(true);
//  	        	           close_news_button.setBackgroundResource(R.drawable.assistant_button);
//            			   //button_panel.setBackgroundResource(R.drawable.light_grey_border);
//            			   
//            			   //LinearLayout.LayoutParams news_mode_params = (LinearLayout.LayoutParams)news_mode_button.getLayoutParams();
//    	        		   //news_mode_params.setMargins(0, 0, 0, 0); //substitute parameters for left, top, right, bottom
//    	        		   //news_mode_button.setLayoutParams(news_mode_params);
//            			   
//            			   layoutMain.removeView(layoutLeft);
//          			       layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
//          			       //news_mode_button.setText("Enter News Mode");
//    	        		   //close_button.setY(10);
//    	        		  //layoutRight.setVisibility(View.VISIBLE);
//          			       layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height );
//    	        		  
//            		   }
            	   }
            	});
            

            
            
//         //----------------------------------------------------------------------------------- 
//       
         //   View rootView = inflate.inflate(R.layout.fragment_main,null);
            
//            FrameLayout layout = (FrameLayout) layoutRight.findViewById( R.id.framelayout );
//            LayoutParams lp = new LayoutParams (LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
//           layout.addView(mUnityPlayer, 0, lp);
//            layoutMain.addView(layoutRight,FrameLayout.LayoutParams.MATCH_PARENT, 800 );
//            mUnityPlayer.resume();
//        
//        
        
        
        mUiHandler = new UIHandler(this);
        I13N.get().registerSession(this);
        App.get().registerUIHandler(mUiHandler);
        App.get().getDataHandler().registerUiHandler(mUiHandler);
         
        System.out.println("#############$$$$$$$$$$$$$$$$");       
        vhmsg=new VhmsgWrapper();
        vhmsg.openConnection();
        String vrSpeak="Brad User 1303332588320-128-1\n"
				+ "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<act>\n"
				+ "<participant id=\"Brad\" role=\"actor\" />\n<bml>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"RIGHT\" angle=\"30\" start=\"0\" ready=\"3.5\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"LEFT\" angle=\"10\" start=\"3.5\" ready=\"4\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<gaze participant=\"Brad\" target=\"Brad\" direction=\"LEFT\" angle=\"0\" start=\"4\" ready=\"5\" openSetItem=\"EYES\" xmlns:sbm=\"http://ict.usc.ed\"/>"
				+ "<head id=\"behavior1\" type = \"NOD\" repetition= \"1\" amount = \"0.3\" start= \"3\"/>\n"
				+ "<animation id = \"animation1\" start=\"sp1:T0\" priority=\"5\" name=\"ChrBrad@Entrance\" />"
				+ "\n</bml>\n</act>";
        vhmsg.send("vrSpeak", vrSpeak);
        String vrExpress="Brad user 1404332904389-10-1  \n<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?> \n<act> \n<participant id=\"Brad\" role=\"actor\" /> \n<fml> \n<turn start=\"take\" end=\"give\" /> \n<affect type=\"neutral\" target=\"addressee\"> \n</affect> \n<culture type=\"neutral\"> \n</culture> \n<personality type=\"neutral\"> \n</personality> \n</fml> \n<bml> \n<speech id=\"sp1\" type=\"application/ssml+xml\">Nice to meet you! I am Brad\n</speech> "
				+"\n</bml> \n</act>";
vhmsg.send("vrExpress", vrExpress);
       // App.mApp.getDataHandler()
      //  vhmsg.closeConnection();
        System.out.println("#############$$$$$$$$$$$$$$$$");
        
        // enable ActionBar app icon to behave as action to toggle nav drawer
        getActionBar().setDisplayHomeAsUpEnabled(true);
        getActionBar().setHomeButtonEnabled(true);
        
        mDm = new DrawerManager(this);
        mDm.onCreateDrawer(savedInstanceState);
        
        //Select/create the first Fragment
        if (savedInstanceState == null) 
        {
        	mUiHandler.post(new Runnable(){

				@Override
				public void run() {
					mDm.selectItem(DrawerManager.DRAWER_DEFAULT);//implicitly select the first item (which is the default news list)
				}
        		
        	});
        }
    }

    public void onClick(View v) {
        switch (v.getId()){
            case 2000: 
            	//Toast.makeText(getApplicationContext(), "Clicked close!",
            			   //Toast.LENGTH_SHORT).show();
            break;
        }
     }
    
   
	@Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.news_main, menu);
        return super.onCreateOptionsMenu(menu);
    }  
	
	@Override
	public void onBackPressed() {
		moveTaskToBack(true);		
	}

	/* Called whenever we call invalidateOptionsMenu() */
    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        // If the nav drawer is open, hide action items related to the content view
        boolean drawerOpen = mDm.isDrawerOpen();
        menu.findItem(R.id.action_switchview).setIcon(getIconRes());
        menu.findItem(R.id.action_switchview).setVisible(!drawerOpen && App.get().getSettings().isFlipViewEnabled());
        menu.findItem(R.id.action_switchview).setEnabled(!bSwitchDisabled);
        
        Button assistant_button = (Button) findViewById(2000);
        Button news_button = (Button) findViewById(2001);
        Button news_mode_button = (Button) findViewById(2002);
        if (drawerOpen) {
        	assistant_button.setEnabled(false);
        	news_button.setEnabled(false);
            news_mode_button.setEnabled(false);
        } else {
        	assistant_button.setEnabled(true);
        	news_button.setEnabled(true);
            news_mode_button.setEnabled(true);
        }
        
        
        return super.onPrepareOptionsMenu(menu);
    }
    
    public void onWindowFocusChanged(boolean hasFocus)
    {
        super.onWindowFocusChanged(hasFocus);
        mUnityPlayer.windowFocusChanged(hasFocus);
    }    
    
    public static class PlaceholderFragment extends Fragment {

        public PlaceholderFragment() {
        }

        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_main, container, false);
            
            FrameLayout layout = (FrameLayout) rootView.findViewById( R.id.framelayout );
            LayoutParams lp = new LayoutParams (LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
            layout.addView(mUnityPlayer, 0, lp);
            
            mUnityPlayer.resume();
            
            return rootView;
        }
    }

    @Override
	protected void onResume() {
		super.onResume();
		invalidateOptionsMenu();
	}


    
	public int getIconRes() {
		return mIconResSwitchView;
	}

	public void setIconRes(int iconResSwitchView) {
		this.mIconResSwitchView = iconResSwitchView;
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent intent) {		
		super.onActivityResult(requestCode, resultCode, intent);
		switch(resultCode){
			case RESULT_OK:
				if (intent.getAction().equals(LoginBrowser.LOGIN_SUCCESS))
				{
					if (getCurrentFrag().getItem() == null)//blank fragment
					{
						mDm.selectItem(0);//select the default item
						break;
					}
					getCurrentFrag().getItem().bDirty = true;
					mDm.selectItem(mCurrentFrag.getItem().idx);//reload data
					mDm.updateDrawerUserName();
    				mDm.showDrawerSelectionAndClose(0);//set focus back to the first item after closing the drawer    				
				}
				break;
		}
	}
 	
	public void enableFragment(NewsListFragment frag) {//onCreateView() of the Fragment will then be called		
		FragmentManager fragmentManager = getFragmentManager();
		NewsListFragment currentFrag = getCurrentFrag();
	
		if (currentFrag != null)//Cancel background loading for the Fragment losing focus
		{
			fragmentManager.beginTransaction().hide(currentFrag).commit();
			fragmentManager.beginTransaction().detach(currentFrag).commit();
			currentFrag.getItem().cancelLoadAsync();
			//Only enqueue when "empty" or "different"
			if (que.size() == 0 || !currentFrag.getItem().name.equals(frag.getItem().name))
			{
				if (que.contains(currentFrag))//remove all existing same fragments
					que.remove(currentFrag);
				que.add(currentFrag);
			}
		}
		//switch to the designated fragment
		if (!frag.isAdded())
			fragmentManager.beginTransaction().add(R.id.content_frame, frag).commit();
		fragmentManager.beginTransaction().attach(frag).commit();
		fragmentManager.beginTransaction().show(frag).commit();	
		
		//determine caching
		while ( (!que.isEmpty()) && (que.size() > MAX_CACHED_FRAGMENTS || MemUtil.getMemUsage() >= MAX_MEMORY) )
		{
			currentFrag = que.poll();
			currentFrag.partialFree();			
		}
        
		setCurrentFrag(frag);
		System.gc();
		
		enableSwitchViewDelayed();
	}

	private void enableSwitchViewDelayed() {
		App.get().getUIHandler().postDelayed(new Runnable(){

			@Override
			public void run() {
				bSwitchDisabled = false;
				invalidateOptionsMenu();
			}
    		
    	}, 1500);
	}

	public NewsListFragment getCurrentFrag()
	{
		return mCurrentFrag;
	}
	
	public void setCurrentFrag(NewsListFragment frag) {
		if (frag == null)
			Log.e("inmind", "setCurrentFrag() set to null!");
		mCurrentFrag = frag;
		setIconRes(mCurrentFrag.getLayoutId() == R.layout.fragment_news_flipview?R.drawable.ic_list:R.drawable.ic_flip);
	}
	
    @Override
    public void setTitle(CharSequence title) {
        mDm.setTitle(title);
        getActionBar().setTitle(title);
    }

    /**
     * When using the ActionBarDrawerToggle, you must call it during
     * onPostCreate() and onConfigurationChanged()...
     */

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        mDm.syncState();       
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        mDm.onConfigurationChanged(newConfig);      
    }
   
	public DrawerManager getDrawerManager() {
		return mDm;
	}
	
	

}