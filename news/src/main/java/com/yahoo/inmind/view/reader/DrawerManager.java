package com.yahoo.inmind.view.reader;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.os.Message;
import android.support.v4.app.ActionBarDrawerToggle;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;

import com.yahoo.inmind.control.reader.ReaderController;
import com.yahoo.inmind.view.browser.BaseBrowser;
import com.yahoo.inmind.view.browser.LoginBrowser;
import com.yahoo.inmind.control.cache.ImgLruCacher;
import com.yahoo.inmind.view.handler.DataHandler;
import com.yahoo.inmind.view.handler.NewsHandler;
import com.yahoo.inmind.model.i13n.Event;
import com.yahoo.inmind.control.i13n.I13N;
import com.yahoo.inmind.view.slingstone.SlingstoneRenderer;
import com.yahoo.inmind.model.slingstone.SlingstoneSrc;

public class DrawerManager
{
	protected ArrayList<DrawerItem> mItems = new ArrayList<DrawerItem>();

	private DrawerLayout mDrawerLayout;
	private ListView mDrawerList;
	private ActionBarDrawerToggle mDrawerToggle;

	private CharSequence mDrawerTitle;
	private CharSequence mTitle;
	private ReaderMainActivity mAct = null;
	private String pkgName = this.getClass().getSimpleName();
    private String user;

    private static int[] drawer_icons = {R.drawable.news_ic_launcher, R.drawable.login_square,
            R.drawable.assistant_square, R.drawable.news_square, R.drawable.both_square };

    private static DrawerManager instance;

    public static DrawerManager getInstance( Context context ) {
        if( context != null ) {
            if (instance == null) {
                if (context instanceof ReaderMainActivity) {
                    instance = new DrawerManager((ReaderMainActivity) context);
                } else {
                    instance = new DrawerManager(context);
                }
            } else {
                if (context instanceof ReaderMainActivity) {
                    instance = new DrawerManager((ReaderMainActivity) context);
                }
            }
        }
        return instance;
    }

    public static DrawerManager getInstance(){
        return getInstance( null );
    }

    private DrawerManager(Context act){
        initializeBase( act );
    }

    private DrawerManager(ReaderMainActivity act){
        initializeBase( act );
        initializeUI( act );
    }


    private void initializeUI( ReaderMainActivity act ){
        //Customize drawer items here
        mAct = act;
    }

    private void initializeBase( Context context ){
        //One DrawerItem corresponds to one fragment, and the fragment is created when the item is selected.
        DrawerItem drawerItem = new DrawerItem();

        //The first item is Slingstone news labeled by the user name or the app_name
        //if no user is present.
        //String user = App.get().getCookieStore().getYahooUserName();
        user = ReaderController.getInstance().getCookieStore().getCurrentUserName();
        drawerItem.name = (user == null? context.getString(R.string.news_name) : user);

        SlingstoneSrc ss = ReaderController.getInstance().getSlingstone( context );//new SlingstoneSrc(context);
        ss.setDrawerManager(this);//We will use this reference to add user profile features as drawerItems later.

        //Register a handler here so Source::generateItemsFromProfile() will be called
        //on a thread associated with the handler when the user profile is ready.
        ss.registerProfileReadyHandler(ReaderController.getInstance().getDataHandler(), DataHandler.PROFILE_READY, ss);
        ImgLruCacher cache = new ImgLruCacher(context);//Enable cache
        drawerItem.srcs.add(ss.enableCache(cache));
        drawerItem.renderers.add(new SlingstoneRenderer(context).enableCache(cache));
        addItem(drawerItem);

        //The second item is a login/logout button
        drawerItem = new DrawerItem();
        drawerItem.name = (user == null? ReaderController.get().getString(R.string.news_login) : ReaderController.get().getString(R.string.news_logout));
        if (user == null)//login
        {
            drawerItem.intent = new Intent(ReaderController.get(), LoginBrowser.class);
            drawerItem.intent.putExtra(BaseBrowser.LAUCH_BROWSER_URL, LoginBrowser.loginUrl);
        }
        addItem(drawerItem, 1);

        drawerItem = new DrawerItem();
        drawerItem.name = "Assistant View";
        addItem(drawerItem);

        drawerItem = new DrawerItem();
        drawerItem.name = "News View";
        addItem(drawerItem);

        drawerItem = new DrawerItem();
        drawerItem.name = "Both View";
        addItem(drawerItem);
    }


	public String getDrawerTitle(int i) {
		return mItems.get(i).name;
	}

	public void onCreateDrawer(Bundle savedInstanceState) {
//        Log.e("DrawerManager.onCreateDrawer", "DrawerManager.onCreateDrawer" );
		mDrawerList = (ListView) mAct.findViewById(R.id.news_left_drawer);
		mTitle = mDrawerTitle = mAct.getTitle();
	  	mDrawerLayout = (DrawerLayout) mAct.findViewById(R.id.news_drawer_layout);
	  	// set a custom shadow that overlays the main content when the drawer opens
	  	mDrawerLayout.setDrawerShadow(R.drawable.news_drawer_shadow, GravityCompat.START);
	  	mDrawerList.setOnItemClickListener(new DrawerItemClickListener());

	  	ArrayList<String> titles = new ArrayList<String>();
		for (DrawerItem it : mItems)
			titles.add(it.name);
		String [] drawerTitles = titles.toArray(new String[titles.size()]);

		// set up the drawer's list view with items and click listener
        mDrawerList.setAdapter(new ArrayAdapter<String>(mAct,
                R.layout.news_drawer_list_item, android.R.id.text1, drawerTitles));

	  	// ActionBarDrawerToggle ties together the the proper interactions
	  	// between the sliding drawer and the action bar app icon
	  	mDrawerToggle = new ActionBarDrawerToggle(
	  			mAct,                  /* host Activity */
	  			mDrawerLayout,         /* DrawerLayout object */
	  			R.drawable.news_ic_drawer,  /* nav drawer image to replace 'Up' caret */
	  			R.string.news_drawer_open,  /* "open drawer" description for accessibility */
	  			R.string.news_drawer_close  /* "close drawer" description for accessibility */
	  			)
	  	{
	  		public void onDrawerClosed(View view) {
	  			mAct.getSupportActionBar().setTitle(mTitle);
	  			mAct.invalidateOptionsMenu(); // creates call to onPrepareOptionsMenu()
	  			I13N.get().log(new Event(pkgName, "onDrawerClosed"));
            }

            public void onDrawerOpened(View drawerView) {
                Log.d("DrawerManager", "Number of drawer items: " + Integer.toString(mDrawerList.getCount()));
                for (int index = 0; index < mDrawerList.getCount(); index++) {
                    View row = mDrawerList.getChildAt(index);
                    ImageView imageView = ((ImageView) row.findViewById(R.id.drawer_icon));
                    imageView.setImageResource(drawer_icons[index]);
                }

            	mAct.getSupportActionBar().setTitle(mDrawerTitle);
            	mAct.invalidateOptionsMenu(); // creates call to onPrepareOptionsMenu()
            	I13N.get().log(new Event(pkgName, "onDrawerOpened"));
            }
        };

        mDrawerLayout.setDrawerListener(mDrawerToggle);
	}

	public void onConfigurationChanged(Configuration newConfig) {
		// Pass any configuration change to the drawer toggls
        mDrawerToggle.onConfigurationChanged(newConfig);
	}

	public void syncState() {
		 mDrawerToggle.syncState();
	}

	public void setTitle(CharSequence title) {
		mTitle = title;
	}

	public void updateDrawerUserName() {
		DrawerItem item = mItems.get(0);
		//Get the user name and set as the first item in the drawer
		String user = ReaderController.getInstance().getCookieStore().getCurrentUserName();
		item.name = (user == null? mAct.getString(R.string.news_name) : user);
		mItems.set(0, item);

		//Set the second item as "Login" or "Logout"
		item = mItems.get(1);
		item.name = (user == null? mAct.getString(R.string.news_login) : mAct.getString(R.string.news_logout));
		if (user == null)//login
		{
			item.intent = new Intent(mAct, LoginBrowser.class);
			item.intent.putExtra(BaseBrowser.LAUCH_BROWSER_URL, LoginBrowser.loginUrl);
		}
		else
		{
			item.intent = null;
		}
		mItems.set(1, item);

		postItemNamesToDrawer();
		mAct.setTitle(mAct.getCurrentFrag().getItem().name);
	}

	public void postItemNamesToDrawer() {
		if (mDrawerList != null)
		{
			ArrayList<String> titles = new ArrayList<String>();
			for (DrawerItem it : mItems)
				titles.add(it.name);
			String [] drawerTitles = titles.toArray(new String[titles.size()]);

			// set up the drawer's list view with items and click listener
	        mDrawerList.setAdapter(new ArrayAdapter<String>(mAct,
	                R.layout.news_drawer_list_item, android.R.id.text1, drawerTitles));
	        mDrawerList.invalidateViews();
		}
	}

	   /* The click listener for ListView in the navigation drawer */
    private class DrawerItemClickListener implements ListView.OnItemClickListener {
        @Override
        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
            selectItem(position, -1);
        }
    }

    public final static int DRAWER_DEFAULT = 0;
    public final static int DRAWER_LOGIN = 1;
    public final static int DRAWER_LOGOUT = 2;

    //This function is used to handle selection event in the drawer
    public void selectItem(int pos, int messageId) {

    	if (pos >= mItems.size())
    	{
    		Log.e("inmind", this.getClass().getSimpleName() + ".selectItem() pos Out Of Range!");
    		return;
    	}

    	DrawerItem item = mItems.get(pos);
    	item.idx = pos;

    	//This special case is to allow drawer items to launch activities,
    	//such as the Login button.
    	if (item.intent != null && mAct != null )
    	{
    		item.idx = 0;
    		mAct.setCurrentFrag(mItems.get(0).frag);
			mAct.startActivityForResult(item.intent, 0);//e.g. start Login activity
            showDrawerSelectionAndClose(0);
    		return;
    	}

//        Log.e("DrawerManager.selectItem", "item: " + item.name );
    	if (item.name.equals(ReaderController.get().getString(R.string.news_logout)))
    	{
    		item = mItems.get(0);
    		if (item.frag != null)
    			item.frag.onPause();
			mAct.setCurrentFrag(item.frag);
			I13N.get().logImmediately(new Event(this.getClass().getSimpleName(), "Logout"));
            ReaderController.getInstance().getCookieStore().logout();
    		updateDrawerUserName();
    		item.bDirty = true;
    	}

        // Handle switching views (assistant, news, both)
        // NOTE: news disappears??

        if (item.name.equals(mAct.getString(R.string.assistant_view))) {
            ReaderMainActivity.clicked_assistant_view();
            item.idx = 0;
            mAct.setCurrentFrag(mItems.get(0).frag);
            showDrawerSelectionAndClose(0);
            return;
        }

        if (item.name.equals(mAct.getString(R.string.news_view))) {
            ReaderMainActivity.clicked_news_view();
            item.idx = 0;
            mAct.setCurrentFrag(mItems.get(0).frag);
            showDrawerSelectionAndClose(0);
            return;
        }

        if (item.name.equals(mAct.getString(R.string.both_view))) {
            ReaderMainActivity.clicked_both_view();
            item.idx = 0;
            mAct.setCurrentFrag(mItems.get(0).frag);
            showDrawerSelectionAndClose(0);
            return;
        }

    	Message msg = new Message();
    	msg.obj = item;
    	if (item.bDirty)//Refresh or new fragment.
    	{
    		msg.what = NewsHandler.PREPARE_FOR_DOWNLOAD_DATA;
            msg.arg1 = messageId;
            ReaderController.getInstance().getUIHandler().sendMessage(msg);
    	}
    	else//Don't reload data, just switch focus to the Fragment
    	{
            //Log.e("","DrawerManager.SHOW_FRAGMENT");
    		msg.what = NewsHandler.SHOW_FRAGMENT;
    		mAct.mUiHandler.sendMessage(msg);

    		Message msgo = new Message();
			msgo.what = NewsHandler.FOCUS_FRAGMENT;
			msgo.obj = item.frag;
			mAct.mUiHandler.sendMessage(msgo);
    	}

        if( mDrawerList  != null ) {
            showDrawerSelectionAndClose(item.idx);
        }
    }

    // update selected item and title, then close the drawer
	protected void showDrawerSelectionAndClose(int pos) {
		mDrawerList.invalidateViews();
		mDrawerList.setItemChecked(pos, true);
		setTitle(mItems.get(pos).name);
		mDrawerLayout.closeDrawer(mDrawerList);
	}

	protected boolean isDrawerOpen()
	{
		return mDrawerLayout.isDrawerOpen(mDrawerList);
	}

	protected boolean onOptionsItemSelected(MenuItem item)
	{
		return mDrawerToggle.onOptionsItemSelected(item);
	}

	public ReaderMainActivity getActivity()
	{
		return mAct;
	}

	public void addItem(DrawerItem item) {
		item.parent = this;
		mItems.add(item);
//        Log.e("DrawerManager.addItem", "item: " + item);
	}

    public void addItem(DrawerItem item, int pos) {
        item.parent = this;
        mItems.add(pos, item);
    }

    public List<DrawerItem> getItems(){
        return mItems;
    }

	public void prepareForExtension()
	{
		//Clean up all extended items
		for (int i = 2; i < mItems.size() ; i++)
		{
			DrawerItem item = mItems.get(i);
			item.cancelLoadAsync();
			if (item.frag != null)
				item.frag.clearAdapter();
		}
		mItems = new ArrayList<DrawerItem>(mItems.subList(0, 5));
	}

	public void notifyDatasetChanged()
	{
		((BaseAdapter)mDrawerList.getAdapter()).notifyDataSetChanged();
	}
}// public class DrawerManager
