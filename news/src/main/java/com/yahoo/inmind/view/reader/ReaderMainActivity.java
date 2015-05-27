package com.yahoo.inmind.view.reader;

import java.net.InetAddress;
import java.net.Socket;
import java.util.LinkedList;
import java.util.Queue;

import android.app.Fragment;
import android.app.FragmentManager;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Color;
import android.graphics.Point;
import android.graphics.drawable.AnimationDrawable;
import android.os.Bundle;
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
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import android.app.Dialog;
//import com.android.sbjniapp.VhmsgWrapper;
import com.unity3d.player.UnityPlayer;
import com.yahoo.inmind.control.i13n.I13N;
import com.yahoo.inmind.control.reader.ReaderController;
import com.yahoo.inmind.view.browser.LoginBrowser;
import com.yahoo.inmind.view.handler.NewsHandler;
import com.yahoo.inmind.view.handler.UIHandler;
import com.yahoo.inmind.view.i13n.I13NActivity;
import com.yahoo.inmind.control.news.NewsArticleVector;
import com.yahoo.inmind.control.util.Constants;
import com.yahoo.inmind.control.util.MemUtil;


public class ReaderMainActivity extends I13NActivity{
    protected static final int MAX_MEMORY = 160;
    protected static final int MAX_CACHED_FRAGMENTS = 0;
    protected NewsListFragment mCurrentFrag = null;
    protected NewsHandler mUiHandler;
    protected Queue<NewsListFragment> que = new LinkedList<NewsListFragment>();//Cached pages
    protected int mIconResSwitchView = R.drawable.news_ic_flip;
    protected boolean bSwitchDisabled = true;
    protected FragmentManager fragmentManager;
    protected NewsListFragment currentFrag;
    private ReaderController reader;

    private static String server_ip = "128.237.221.118";
    private EditText newIP;
    private DrawerManager mDm = null;
//    private static UnityPlayer mUnityPlayer;
   // public static VhmsgWrapper vhmsg = null;
    public static boolean assistant_button_clicked = false;
    public static boolean news_button_clicked = false;
    public static boolean news_mode_clicked = false;
    static Context context;
    static Button news_button;
    static Button assistant_button;
    static Button news_mode_button;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.news_outside_layout);
        context = this;

        // enable ActionBar app icon to behave as action to toggle nav drawer
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
            getSupportActionBar().setHomeButtonEnabled(true);
        } else {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
            getSupportActionBar().setHomeButtonEnabled(true);
        }
        getSupportActionBar().setLogo(R.drawable.news_ic_launcher);
        getSupportActionBar().setDisplayUseLogoEnabled(true);
        reader = ReaderController.getInstance();
        ReaderController.setFlagRefreshAsyncListView(true);


        // Override all the UI components that you have been customized
        setLandscapeLayout((Integer) getIntent().getExtras().get(Constants.UI_LANDSCAPE_LAYOUT));
        setPortraitLayout((Integer) getIntent().getExtras().get(Constants.UI_PORTRAIT_LAYOUT));
        setUINewsRank((Integer) getIntent().getExtras().get(Constants.UI_NEWS_RANK));
        setUINewsTitle((Integer) getIntent().getExtras().get(Constants.UI_NEWS_TITLE));
        setUINewsSummary((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SUMMARY));
        setUINewsImg((Integer) getIntent().getExtras().get(Constants.UI_NEWS_IMG));
        setUINewsPublisher((Integer) getIntent().getExtras().get(Constants.UI_NEWS_PUBLISHER));
        setUINewsReason((Integer) getIntent().getExtras().get(Constants.UI_NEWS_REASON));
        setUINewsScore((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SCORE));
        setUINewsFeat((Integer) getIntent().getExtras().get(Constants.UI_NEWS_FEAT));
        setUINewsFeat2((Integer) getIntent().getExtras().get(Constants.UI_NEWS_FEAT2));
        setUINewsShareFB((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SHARE_FB));
        setUINewsShareTwitter((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SHARE_TWITTER));
        setUINewsShareTumblr((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SHARE_TMBLR));
        setUINewsShareMore((Integer) getIntent().getExtras().get(Constants.UI_NEWS_SHARE_MORE));
        setUINewsLike((Integer) getIntent().getExtras().get(Constants.UI_NEWS_LIKE));
        setUINewsDislike((Integer) getIntent().getExtras().get(Constants.UI_NEWS_DISLIKE));
        setUINewsComments((Integer) getIntent().getExtras().get(Constants.UI_NEWS_COMMENTS));

        reader.initialize(this, savedInstanceState, -1);
        //reader.messageId = -1;
        mUiHandler = reader.getUIHandler();
        NewsArticleVector.initialize();

////
        Display display = getWindowManager().getDefaultDisplay();
        Point size = new Point();
        display.getSize(size);
        final int width = size.x;
        final int height = size.y;
        final int button_margin = 10;
        final int news_height = height/2;
        final int assistant_height = height - news_height - 200;
        final int small_assistant_height = height/3;
        final int small_assistant_width = width/4;
        final int assistant_button_width = width/3 - button_margin;
        final int news_button_width = width/3 - button_margin;
        final int news_mode_button_width = width/3 - button_margin;
        final int button_panel_height = height/16;


        // EZ: Created an "outside" layout that contains all the other layouts, so that
        // it can also have a sliding drawer
        DrawerLayout layoutOutside = (DrawerLayout) findViewById(R.id.news_drawer_layout);
        final FrameLayout layoutFrame = (FrameLayout) findViewById(R.id.news_layout_frame);
        final RelativeLayout layoutRel = (RelativeLayout) findViewById(R.id.news_layout_rel);
        final LinearLayout layoutMain = (LinearLayout) findViewById(R.id.news_layout_main);
        layoutMain.setBackgroundColor(Color.parseColor("#000000"));
        layoutMain.setOrientation(LinearLayout.VERTICAL);

        setContentView(layoutOutside);

        // Button panel holds 'tabs' for switching views
        // NOTE: this is currently not visible (it is from an older version)
        // It is still here to preserve the onClickListeners, and in case we wish to use it again
        final LinearLayout button_panel = new LinearLayout(this);
        button_panel.setOrientation(LinearLayout.HORIZONTAL);
        button_panel.setBackgroundColor(Color.parseColor("#000000"));
        //layoutMain.addView(button_panel, LinearLayout.LayoutParams.MATCH_PARENT, button_panel_height);

        final LayoutInflater inflate = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        // Assistant button changes view to full screen assistant
        assistant_button = new Button(this);
        assistant_button.setTextSize(12);
        assistant_button.setTextColor(Color.parseColor("#000000"));
        assistant_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
        assistant_button.setBackgroundResource(R.drawable.assistant_button);
        LinearLayout.LayoutParams assistant_button_params = new LinearLayout.LayoutParams(assistant_button_width,
                button_panel_height);
        assistant_button_params.setMargins(5, 5, 0, 5);
        assistant_button.setLayoutParams(assistant_button_params);

        button_panel.addView(assistant_button, assistant_button_params);

        // News button changes view to full screen news app
        news_button = new Button(this);
        news_button.setTextSize(12);
        news_button.setTextColor(Color.parseColor("#000000"));
        news_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
        news_button.setBackgroundResource(R.drawable.news_button);
        LinearLayout.LayoutParams news_button_params = new LinearLayout.LayoutParams(news_button_width,
                button_panel_height);
        news_button_params.setMargins(10, 5, 0, 5);
        news_button.setLayoutParams(news_button_params);
        button_panel.addView(news_button, news_button_params);

        // News mode changes view to assistant on top of news
        news_mode_button = new Button(this);
        news_mode_button.setTextSize(12);
        news_mode_button.setTextColor(Color.parseColor("#000000"));
        news_mode_button.setBackgroundColor(Color.parseColor("#0ABEF5"));
        news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
        LinearLayout.LayoutParams news_mode_button_params = new LinearLayout.LayoutParams(
                news_mode_button_width, button_panel_height);
        news_mode_button_params.setMargins(10, 5, 10, 5);
        news_mode_button.setLayoutParams(news_mode_button_params);
        button_panel.addView(news_mode_button, news_mode_button_params);


        // Add news app View
        final LinearLayout layoutLeft = (LinearLayout) inflate.inflate(
                R.layout.news_main, null);
        LinearLayout.LayoutParams layout_left_params = new LinearLayout.LayoutParams(
                width, news_height);
        layoutMain.addView(layoutLeft, layout_left_params);

        // TEMPORARY REPLACEMENT FOR UNITY VIEW
        final RelativeLayout layoutRight = new RelativeLayout(this);

//        // Add assistant View
//        mUnityPlayer = new UnityPlayer(this);
//        int glesMode = mUnityPlayer.getSettings().getInt("gles_mode", 1);
//        mUnityPlayer.init(glesMode, false);
//
//        final RelativeLayout layoutRight = (RelativeLayout) inflate.inflate(
//                R.layout.news_fragment_main, null);
//        RelativeLayout.LayoutParams relParam = new RelativeLayout.LayoutParams(
//                RelativeLayout.LayoutParams.WRAP_CONTENT,
//                RelativeLayout.LayoutParams.WRAP_CONTENT);

        layoutMain.addView(layoutRight,RelativeLayout.LayoutParams.MATCH_PARENT, assistant_height);


        // Add static mic button, layered on top with a frame layout
        final Button mic_button = new Button(this);
        mic_button.setBackgroundResource(R.drawable.mic_button_selector);
        RelativeLayout.LayoutParams mic_button_params = new RelativeLayout.LayoutParams(220, 150);
        mic_button.setLayoutParams(mic_button_params);
        mic_button_params.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
        //mic_button_params.addRule(RelativeLayout.CENTER_HORIZONTAL);
        layoutRel.addView(mic_button, mic_button_params);


        // The following are three onClickListeners for the three view tabs,
        // so that they are mutually exclusive buttons
        assistant_button.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                assistant_button_clicked = true;
                assistant_button.setBackgroundResource(R.drawable.assistant_button_pressed);
                if (news_button_clicked) {
                    news_button_clicked = false;
                    news_button.setBackgroundResource(R.drawable.news_button);
                    layoutMain.removeView(layoutLeft);
                    layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
                    layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
                }
                if (news_mode_clicked) {
                    news_mode_clicked = false;
                    news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
                    layoutMain.removeView(layoutRight);
                    layoutMain.removeView(layoutLeft);
                    layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height);
                    layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
                }
                layoutLeft.setVisibility(View.GONE);
                layoutMain.removeView(layoutRight);
                layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, height - 200);

            }
        });

        news_button.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                news_button_clicked = true;
                news_button.setBackgroundResource(R.drawable.news_button_pressed);
                if (assistant_button_clicked) {
                    assistant_button_clicked = false;
                    assistant_button.setBackgroundResource(R.drawable.assistant_button);
                    layoutMain.removeView(layoutRight);
                    layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
                    layoutLeft.setVisibility(View.VISIBLE);
                }
                if (news_mode_clicked) {
                    news_mode_clicked = false;
                    news_mode_button.setBackgroundResource(R.drawable.news_mode_button);
                    layoutMain.removeView(layoutRight);
                    layoutMain.removeView(layoutLeft);
                    layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, news_height );
                    layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
                }
                layoutMain.removeView(layoutRight);
                layoutMain.removeView(layoutLeft);
                layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, height);

            }
        });


        news_mode_button.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
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
                    layoutMain.addView(layoutRight,DrawerLayout.LayoutParams.MATCH_PARENT, assistant_height);
                }
                layoutMain.removeView(layoutRight);
                layoutMain.removeView(layoutLeft);

                LinearLayout.LayoutParams small_assistant_params = new LinearLayout.LayoutParams(
                        LinearLayout.LayoutParams.MATCH_PARENT, small_assistant_height);
                layoutRight.setLayoutParams(small_assistant_params);

                layoutMain.addView(layoutRight, small_assistant_params);
                layoutMain.addView(layoutLeft,DrawerLayout.LayoutParams.MATCH_PARENT, height - small_assistant_height - 200);

            }
        });

        // Let the initial view be the assistant view
      // assistant_button.performClick();

       // enable ActionBar app icon to behave as action to toggle nav drawer
//        getActionBar().setDisplayHomeAsUpEnabled(true);
//        getActionBar().setHomeButtonEnabled(true);



       DrawerManager.getInstance().onCreateDrawer(savedInstanceState);

      //  Select/create the first Fragment
        if (savedInstanceState == null)
        {
            mUiHandler.post(new Runnable(){

                @Override
                public void run() {
                    DrawerManager.getInstance().selectItem(0, -1);//implicitly select the first item (which is the default news list)
                }

            });
        }

    }

    ////////////////// try? ////////////////////
    public boolean onTouch(View v, MotionEvent event) {
        int x = (int)event.getX();
        int y = (int)event.getY();
        Log.d("TouchEvent", "(" + Integer.toString(x) + ", " + Integer.toString(y) + ")");
        return true;
    }


    public void setPortraitLayout( Integer portraitLayout ){
        reader.portrait_layout = portraitLayout;
    }

    public void setLandscapeLayout( Integer landscapeLayout ){
        reader.landscape_layout = landscapeLayout;
    }

    public void setUINewsRank( Integer uiNewsRank ){
        reader.news_rank = uiNewsRank;
    }

    public void setUINewsTitle( Integer uiNewsTitle ){
        reader.news_title = uiNewsTitle;
    }

    public void setUINewsScore( Integer uiNewsScore ){
        reader.news_score = uiNewsScore;
    }

    public void setUINewsSummary( Integer uiNewsSummary ){
        reader.news_summary = uiNewsSummary;
    }

    public void setUINewsFeat( Integer uiNewsFeat ){
        reader.news_feat = uiNewsFeat;
    }

    public void setUINewsFeat2( Integer uiNewsFeat2 ){
        reader.news_feat2 = uiNewsFeat2;
    }

    public void setUINewsPublisher( Integer uiNewsPublisher ){
        reader.news_publisher = uiNewsPublisher;
    }

    public void setUINewsReason( Integer uiNewsReason ){
        reader.news_reason = uiNewsReason;
    }

    public void setUINewsImg( Integer uiNewsImg ){
        reader.news_img = uiNewsImg;
    }

    public void setUINewsShareFB( Integer uiNewsShareFB ){
        reader.setNews_btnShareFb( uiNewsShareFB );
    }

    public void setUINewsShareTwitter( Integer uiNewsShareTwitter ){
        reader.setNews_btnShareTwitter( uiNewsShareTwitter );
    }

    public void setUINewsShareTumblr( Integer uiNewsShareTumblr ){
        reader.setNews_btnShareTumblr( uiNewsShareTumblr );
    }

    public void setUINewsShareMore( Integer uiNewsShareMore ){
        reader.setNews_btnShareMore( uiNewsShareMore );
    }

    public void setUINewsDislike( Integer uiNewsDislike ){
        reader.setNews_btnDislike( uiNewsDislike );
    }

    public void setUINewsComments( Integer uiNewsComments ){
        reader.news_comments = uiNewsComments;
    }

    public void setUINewsLike( Integer uiNewsLike ){
        reader.setNews_btnLike( uiNewsLike );
    }


    // Handle switching views from the slide out drawer
    // NOTE: drawer only appears in the news window, so once you switch to assistant view, you
    // cannot pull out the drawer
    public static void clicked_news_view() {
        Toast.makeText(context, "NEWS VIEW",
                Toast.LENGTH_SHORT).show();
        news_button.performClick();
    }

    public static void clicked_assistant_view() {
        Toast.makeText(context, "ASSISTANT VIEW",
                Toast.LENGTH_SHORT).show();
        assistant_button.performClick();
    }

    public static void clicked_both_view() {
        Toast.makeText(context, "BOTH VIEW",
                Toast.LENGTH_SHORT).show();
        news_mode_button.performClick();
    }


	@Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.news_main, menu);
        return super.onCreateOptionsMenu(menu);
    }

	@Override
	public void onBackPressed() {
        //NewsArticleVector.storeNewsOnPhone( NewsArticleVector.getVisitedList() );
        reader.getSlingstone( null ).cancelTasks();
        super.onBackPressed();

        //moveTaskToBack(true);
    }


    @Override
    protected void onDestroy(){
        super.onDestroy();
        //super.recreate();
    }


    @Override
    protected void onPause(){
        super.onPause();
    }


    /* Called whenever we call invalidateOptionsMenu() */
    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        // If the nav drawer is open, hide action items related to the content view
        boolean drawerOpen = DrawerManager.getInstance().isDrawerOpen();
        menu.findItem(R.id.news_action_switchview).setIcon(getIconRes());
        menu.findItem(R.id.news_action_switchview).setVisible(!drawerOpen && reader.getSettings().isFlipViewEnabled());
        menu.findItem(R.id.news_action_switchview).setEnabled(!bSwitchDisabled);
        return super.onPrepareOptionsMenu(menu);
    }

    public void onWindowFocusChanged(boolean hasFocus)
    {
        super.onWindowFocusChanged(hasFocus);
//        mUnityPlayer.windowFocusChanged(hasFocus);
        if (hasFocus) {
            // Starting the animation when in Focus
            //frameAnimation.start();
        } else {
            // Stopping the animation when not in Focus
//        	frameAnimation.stop();
//        	LinearLayout layoutMain = (LinearLayout)findViewById(2003);
//        	ImageView anim = (ImageView) findViewById(2004);
//        	layoutMain.removeView(anim);


        }
    }

//    public static class PlaceholderFragment extends Fragment {
//
//        public PlaceholderFragment() {
//        }
//
//        @Override
//        public View onCreateView(LayoutInflater inflater, ViewGroup container,
//                                 Bundle savedInstanceState) {
//            View rootView = inflater.inflate(R.layout.news_fragment_main, container, false);
//
//            FrameLayout layout = (FrameLayout) rootView.findViewById( R.id.framelayout );
//            FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(
//                    FrameLayout.LayoutParams.FILL_PARENT, FrameLayout.LayoutParams.FILL_PARENT);
//            layout.addView(mUnityPlayer, 0, lp);
//
//            mUnityPlayer.resume();
//
//            return rootView;
//        }
//    }

    @Override
	protected void onResume() {
		super.onResume();
		invalidateOptionsMenu();
	}

	@Override
    public boolean onOptionsItemSelected(MenuItem item) {
         // The action bar home/up action should open or close the drawer.
         // ActionBarDrawerToggle will take care of this.
        if (DrawerManager.getInstance().onOptionsItemSelected(item)){
            return true;
        }
        int itemId = item.getItemId();
        if (itemId == R.id.action_changeIP) {
            // Opens a dialog box for input of new IP address
            final Dialog dialog = new Dialog(this);
            dialog.setContentView(R.layout.ip_dialog);
            dialog.setTitle("Edit IP Address");
            TextView text = (TextView) dialog.findViewById(R.id.enter_ip_text);
            text.setText("Enter new IP address: ");
            Button dialogButton = (Button) dialog.findViewById(R.id.enter_ip_OK);
            newIP = (EditText) dialog.findViewById(R.id.enter_ip);

            // if OK button is clicked, close the custom dialog
            dialogButton.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    if (newIP.getText().toString() != " ") {
                        server_ip = newIP.getText().toString();
                        Toast.makeText(getApplicationContext(), server_ip,
                                Toast.LENGTH_LONG).show();
                    }
                    dialog.dismiss();
                }
            });

            dialog.show();
            return true;
        } else if (itemId == R.id.news_action_switchview) {
			NewsListFragment frag = new NewsListFragment(getCurrentFrag().getItem());
			//Switch the layouts
			frag.setLayoutId(getCurrentFrag().getLayoutId() == R.layout.news_fragment_flipview ?
					R.layout.news_fragment_listview : R.layout.news_fragment_flipview);
			frag.getItem().bDirty = false;
			frag.getItem().bklist.addAll(frag.getItem().list);
			frag.getItem().list.clear();//to prevent listItem be freed when the old frag is removed -> destroyed
			frag.getItem().frag = frag;
			frag.setLastItemIdx(getCurrentFrag().getLastItemIdx());
			frag.setScrollToPos(getCurrentFrag().getLastItemIdx());
			enableFragment(frag);
			mUiHandler.sendEmptyMessage(NewsHandler.SHOW_LOADING_COMPLETE);
			int layoutId = frag.getLayoutId();
			if (layoutId == R.layout.news_fragment_flipview) {
				setIconRes(R.drawable.news_ic_list);
			} else if (layoutId == R.layout.news_fragment_listview) {
				setIconRes(R.drawable.news_ic_flip);
			}
			bSwitchDisabled = true;
			enableSwitchViewDelayed();
			invalidateOptionsMenu();
			return true;
		} else if (itemId == R.id.news_action_settings) {
            startActivity(new Intent(this, SettingsActivity.class));
			return true;
		} else if( itemId == R.id.news_action_refresh ){
            Toast toast = Toast.makeText( this, "Next article...", Toast.LENGTH_SHORT );
            toast.setGravity(Gravity.TOP, 0, 0);
            toast.show();

            // we need this otherwise doesn't process the last article of the screen
            while( NewsArticleVector.getEndPosBatch()
                    - (reader.getListView().getCurrentArticle()
                    + NewsArticleVector.getStartPosBatch() ) > 2 ) {
                NewsArticleVector.increaseCurrentPosition();
                reader.getListView().increaseCurrentArticle();
            }

            NewsArticleVector.processCurrentArticle(true, true);
            ReaderController.getListView().resetValues();
            return true;
        } else {
			return super.onOptionsItemSelected(item);
		}
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
                if( intent.getAction() != null ) {
                    if (intent.getAction().equals(LoginBrowser.LOGIN_SUCCESS)) {
                        if (getCurrentFrag().getItem() == null)//blank fragment
                        {
                            DrawerManager.getInstance().selectItem(0, -1);//select the default item
                            break;
                        }
                        getCurrentFrag().getItem().bDirty = true;
                        DrawerManager.getInstance().selectItem(mCurrentFrag.getItem().idx, -1);//reload data
                        DrawerManager.getInstance().updateDrawerUserName();
                        DrawerManager.getInstance().showDrawerSelectionAndClose(0);//set focus back to the first item after closing the drawer
                    }
                }
				break;
		}
	}

	public void enableFragment(NewsListFragment frag) {//onCreateView() of the Fragment will then be called
        fragmentManager = getFragmentManager();
        currentFrag = getCurrentFrag();

		if (currentFrag != null)//Cancel background loading for the Fragment losing focus
		{
			fragmentManager.beginTransaction().hide(currentFrag).commitAllowingStateLoss();
			fragmentManager.beginTransaction().detach(currentFrag).commitAllowingStateLoss();
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
			fragmentManager.beginTransaction().add(R.id.news_content_frame, frag).commitAllowingStateLoss();
		fragmentManager.beginTransaction().attach(frag).commitAllowingStateLoss();
		fragmentManager.beginTransaction().show(frag).commitAllowingStateLoss();

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
        reader.getUIHandler().postDelayed(new Runnable(){

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
		setIconRes(mCurrentFrag.getLayoutId() == R.layout.news_fragment_flipview ?R.drawable.news_ic_list :R.drawable.news_ic_flip);
	}

    @Override
    public void setTitle(CharSequence title) {
        DrawerManager.getInstance().setTitle(title);
        getSupportActionBar().setTitle(title);
    }

    /**
     * When using the ActionBarDrawerToggle, you must call it during
     * onPostCreate() and onConfigurationChanged()...
     */

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        DrawerManager.getInstance().syncState();
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        DrawerManager.getInstance().onConfigurationChanged(newConfig);

    }

	public DrawerManager getDrawerManager() {
		return DrawerManager.getInstance();
	}
}