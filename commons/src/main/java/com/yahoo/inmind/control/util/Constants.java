package com.yahoo.inmind.control.util;

/**
 * Created by oscarr on 12/18/14.
 */
public final class Constants {
    /** Messages **/
    public static final int MSG_LAUNCH_BASE_NEWS_ACTIVITY = 1;
    public static final int MSG_LAUNCH_EXT_NEWS_ACTIVITY = 2;
    public static final int MSG_REQUEST_NEWS_ITEMS = 3;
    public static final int MSG_SHOW_MODIFIED_NEWS = 4;
    public static final int MSG_LOGIN = 5;
    public static final int MSG_GET_USER_PROFILE = 6;
    public static final int MSG_UPDATE_NEWS = 7;
    public static final int MSG_GET_NEWS_ITEMS = 8;
    public static final int MSG_APPLY_FILTERS = 9;
    public static final int MSG_SHOW_ARTICLE = 10;
    public static final int MSG_EXPAND_ARTICLE = 11;
    public static final int MSG_LAUNCH_ACTIVITY = 12;
    public static final int MSG_GET_ARTICLE_POSITION = 13;
    public static final int MSG_SHOW_CURRENT_ARTICLE = 14;
    public static final int MSG_SHOW_NEXT_ARTICLE = 15;
    public static final int MSG_SHOW_PREVIOUS_ARTICLE = 16;
    public static final int MSG_START_AUDIO_RECORD = 17;
    public static final int MSG_STOP_AUDIO_RECORD = 18;
    public static final int MSG_UPLOAD_TO_SERVER = 19;



    /** Bundle fields **/
    public static final String BUNDLE_ACTIVITY_NAME = "BUNDLE_ACTIVITY_NAME";
    public static final String BUNDLE_MODIFIED_NEWS = "BUNDLE_MODIFIED_NEWS";
    public static final String BUNDLE_FILTERS = "BUNDLE_FILTERS";
    public static final String BUNDLE_ARTICLE_ID = "BUNDLE_ARTICLE_ID";

    /** qualifiers **/
    public static final String QUALIFIER_NEWS = "QUALIFIER_NEWS";

    /** Flags **/
    public static final java.lang.String FLAG_FORCE_RELOAD = "FORCE_RELOAD";
    public static final String FLAG_RETURN_JSON = "FLAG_RETURN_JSON";
    public static final String FLAG_UPDATE_NEWS = "FLAG_UPDATE_NEWS";
    public static final String FLAG_SEND_EVENT = "FLAG_SEND_EVENT";
    public static final String FLAG_REFRESH = "FLAG_REFRESH";


    /** layouts **/
    public static final String UI_PORTRAIT_LAYOUT = "UI_PORTRAIT_LAYOUT";
    public static final String UI_LANDSCAPE_LAYOUT = "UI_LANDSCAPE_LAYOUT";
    public static final String UI_NEWS_RANK = "UI_NEWS_RANK";
    public static final String UI_NEWS_TITLE = "UI_NEWS_TITLE";
    public static final String UI_NEWS_SCORE = "UI_NEWS_SCORE";
    public static final String UI_NEWS_SUMMARY = "UI_NEWS_SUMMARY";
    public static final String UI_NEWS_FEAT = "UI_NEWS_FEAT";
    public static final String UI_NEWS_FEAT2 = "UI_NEWS_FEAT2";
    public static final String UI_NEWS_PUBLISHER = "UI_NEWS_PUBLISHER";
    public static final String UI_NEWS_REASON = "UI_NEWS_REASON";
    public static final String UI_NEWS_IMG = "UI_NEWS_IMG";
    public static final String UI_NEWS_SHARE_FB = "UI_NEWS_SHARE_FB";
    public static final String UI_NEWS_SHARE_TWITTER = "UI_NEWS_SHARE_TWITTER";
    public static final String UI_NEWS_SHARE_TMBLR = "UI_NEWS_SHARE_TMBLR";
    public static final String UI_NEWS_SHARE_MORE = "UI_NEWS_SHARE_MORE";
    public static final String UI_NEWS_LIKE = "UI_NEWS_LIKE";
    public static final String UI_NEWS_DISLIKE = "UI_NEWS_DISLIKE";
    public static final String UI_NEWS_COMMENTS = "UI_NEWS_COMMENTS";


    /** login **/
    public static final String RESULTS_LOGIN = "RESULTS_LOGIN";

    /** news reader **/
    //Constants of JSON paths for properties of user profile
    public static final String JSON_YAHOO_USER_PROFILE_PATH = "yahoo-coke:*/yahoo-coke:debug-scoring/feature-response/result";
    public static final String JSON_USER_GENDER = "JSON_USER_GENDER";
    public static final String JSON_USER_AGE = "JSON_USER_AGE";
    public static final String JSON_POSITIVE_DEC_WIKIID = "POSITIVE_DEC WIKIID";
    public static final String JSON_POSITIVE_DEC_YCT = "POSITIVE_DEC YCT";
    public static final String JSON_NEGATIVE_DEC_WIKIID = "NEGATIVE_DEC WIKIID";
    public static final String JSON_NEGATIVE_DEC_YCT = "NEGATIVE_DEC YCT";
    public static final String JSON_FB_WIKIID = "FB WIKIID";
    public static final String JSON_FB_YCT = "FB YCT";
    public static final String JSON_CAP_ENTITY_WIKI = "JSON_CAP_ENTITY_WIKI";
    public static final String JSON_CAP_YCT_ID = "JSON_CAP_YCT_ID";
    public static final String JSON_NEGATIVE_INF_WIKIID = "NEGATIVE_INF WIKIID";
    public static final String JSON_NEGATIVE_INF_YCT = "NEGATIVE_INF YCT";
    public static final String JSON_USER_PROPUSAGE = "JSON_USER_PROPUSAGE";
    //Constants of JSON paths for properties of news content
    public static final String JSON_YAHOO_COKE_STREAM_ELEMENTS = "yahoo-coke:stream/elements";
    public static final String JSON_SS_FAKE_USER_PROFILE_PARAM_NAME = "profile";

    /** news item attributes **/
    public static final String ARTICLE_INDEX = "index";
    public static final String ARTICLE_TITLE = "title";
    public static final String ARTICLE_UUID = "uuid";
    public static final String ARTICLE_REASON = "explain/reason";
    public static final String ARTICLE_URL = "snippet/url";
    public static final String ARTICLE_CATEGORIES = "snippet/categories";
    public static final String ARTICLE_SCORE = "score";
    public static final String ARTICLE_CAP_FEATURES = "cap_features";
    public static final String ARTICLE_RAW_SCORE_MAP = "raw_score_map";
    public static final String ARTICLE_PUBLISHER = "publisher";
    public static final String ARTICLE_IMAGE_URL = "snippet/image/original/url";
    public static final String ARTICLE_SUMMARY = "snippet/summary";
    public static final String ARTICLE_PREVIOUS_POSITION = "ARTICLE_PREVIOUS_POSITION";
    public static final String ARTICLE_NEXT_POSITION = "ARTICLE_NEXT_POSITION";

    /** news filters **/
    public static final String FILTER_HIGHER_THAN = "FILTER_HIGHER_THAN";
    public static final String FILTER_EQUALS_TO = "FILTER_EQUALS_TO";
    public static final String FILTER_LOWER_THAN = "FILTER_LOWER_THAN";
    public static final String FILTER_CONTAINS_STRING = "FILTER_CONTAINS_STRING";

    /** Contents **/
    public static final String CONTENT_NEWS_LIST = "CONTENT_NEWS_LIST";
    public static final String CONTENT = "CONTENT";

    /** Set constant values **/
    public static final int SET_NEWS_LIST_SIZE = 0;
    public static final int SET_REFRESH_TIME = 1;
    public static final int SET_UPDATE_TIME = 2;
    public static final String SET_AUDIO_SAMPLE_RATE = "SET_AUDIO_SAMPLE_RATE";
    public static final String SET_AUDIO_CHANNEL_CONFIG = "SET_AUDIO_CHANNEL_CONFIG";
    public static final String SET_AUDIO_ENCODING = "SET_AUDIO_ENCODING";
    public static final String SET_SUBSCRIBER = "SET_SUBSCRIBER";
    public static final String SET_AUDIO_BUFFER_ELEMENTS_TO_REC = "SET_AUDIO_BUFFER_ELEMENTS_TO_REC";
    public static final String SET_AUDIO_BYTES_PER_ELEMENT = "SET_AUDIO_BYTES_PER_ELEMENT";



    /** Configuration variables **/
    public static final String CONFIG_PERSONALIZATION_MULTIBANDIT_LEARNING = "CONFIG_PERSONALIZATION_MULTIBANDIT_LEARNING";
    public static final String CONFIG_PERSONALIZATION_LEARNING_FROM_ADVISE = "CONFIG_PERSONALIZATION_LEARNING_FROM_ADVISE";
    public static final String CONFIG_FEEDBACK_MULTIBANDIT_LEARNING = "CONFIG_FEEDBACK_MULTIBANDIT_LEARNING";
    public static final String CONFIG_FEEDBACK_LEARNING_FROM_ADVISE = "CONFIG_FEEDBACK_LEARNING_FROM_ADVISE";
    public static final String CONFIG_NEWS_PROPERTIES = "news_config.properties";
    public static final String CONFIG_ID_PERSONALIZATION = "CONFIG_ID_PERSONALIZATION";
    public static final String CONFIG_ID_FEEDBACK = "CONFIG_ID_FEEDBACK";
    public static final String CONFIG_NEWS_RANKING_OPTION = "CONFIG_NEWS_RANKING_OPTION";


    public static final String HTTP_REQUEST_SERVER_URL = "HTTP_REQUEST_SERVER_URL";
    public static final String HTTP_REQUEST_BODY = "HTTP_REQUEST_BODY";


    public static final String IMG_QUALITY = "IMG_QUALITY";
    public static final String IMG_COMPRESS_FORMAT = "IMG_COMPRESS_FORMAT";
    public static final String IMG_NAME = "IMG_NAME";
    public static final String IMG_YUV_FORMAT = "IMG_YUV_FORMAT";
}
