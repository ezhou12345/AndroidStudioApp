/*
 * Copyright (C) 2009 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// OpenGL ES 2.0 code

#include <jni.h>


#include <vhcl_log.h>
#include <vhcl_string.h>
#include <vhmsg-tt.h>
#include <vhmsg.h>
#include <vhmsg-tt.cpp>



#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#include <android/log.h>


#define  LOG_TAG    "libsbjniapp"
#define  LOGI(...)  __android_log_print(ANDROID_LOG_INFO,LOG_TAG,__VA_ARGS__)
#define  LOGE(...)  __android_log_print(ANDROID_LOG_ERROR,LOG_TAG,__VA_ARGS__)

void initConnection()
{
		
	const char* serverName = "128.237.196.205";
	const char* scope = "DEFAULT_SCOPE";
	const char* port = "61616";
	int err;
	int openConnection = vhmsg::ttu_open(serverName,scope,port);
	if( openConnection == vhmsg::TTU_SUCCESS )
	{
	 //vhmsg::ttu_set_client_callback( sb_vhmsg_callback );
		err = vhmsg::ttu_register( "sb" );
		err = vhmsg::ttu_register( "sbm" );
		err = vhmsg::ttu_register( "vrAgentBML" );
		err = vhmsg::ttu_register( "vrExpress" );
		err = vhmsg::ttu_register( "vrSpeak" );
		err = vhmsg::ttu_register( "RemoteSpeechReply" );
		err = vhmsg::ttu_register( "PlaySound" );
		err = vhmsg::ttu_register( "StopSound" );
		err = vhmsg::ttu_register( "CommAPI" );
		err = vhmsg::ttu_register( "object-data" );
		err = vhmsg::ttu_register( "vrAllCall" );
		err = vhmsg::ttu_register( "vrKillComponent" );
		err = vhmsg::ttu_register( "wsp" );
		err = vhmsg::ttu_register( "receiver" );
		LOGI("TTU Open very Success : server = %s, scope = %s, port = %s",serverName,scope,port);
	}
	else
	{
		LOGE("TTU Open Failed : server = %s, scope = %s, port = %s",serverName,scope,port);
	}
}





extern "C" {
   JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_openConnection( JNIEnv * env, jobject obj);
   JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_closeConnection( JNIEnv * env, jobject obj);
   JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_send( JNIEnv * env, jobject obj,jstring opname,jstring message);
   //JNIEXPORT jlong JNICALL Java_com_android_sbjniapp_VhmsgWrapper_initializeMsg(JNIEnv * env, jobject obj);

 };


JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_send( JNIEnv * env, jobject obj,jstring opname,jstring message){
	const char * op= env->GetStringUTFChars(opname, 0);
	const char* msg= env->GetStringUTFChars(message, 0);
	int call_back=vhmsg::ttu_notify2( op,msg);
	if( call_back == vhmsg::TTU_SUCCESS ){LOGI("send sucessfully!");}

}


JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_closeConnection( JNIEnv * env, jobject obj)
{

  int call_back=vhmsg::ttu_close();

  if( call_back == vhmsg::TTU_SUCCESS ){LOGI("close sucessfully!");}
  
}


JNIEXPORT void JNICALL Java_com_android_sbjniapp_VhmsgWrapper_openConnection( JNIEnv * env, jobject obj)
{
	const char* serverName = "128.237.196.205";
	const char* scope = "DEFAULT_SCOPE";
	const char* port = "61616";
//	vhmsg::Client vm;
//	vm.SetScope( scope );
//	LOGE("TTU Open  Successlalalal1:");
//	vhmsg:: Client* vm=(vhmsg::Client*) lp;
//	vm->SetScope(scope);
//	LOGE("TTU Open  Successlalalal:");
//	bool a= vm->OpenConnection(serverName,port);
	//LOGI("TTU Open  Success: %d", a);
	return initConnection();
	//jstring* a=(jstring*)lp;
//	return *a;
}



#if 0
JNIEXPORT jlong JNICALL Java_com_android_sbjniapp_VhmsgWrapper_initializeMsg(JNIEnv * env, jobject obj)
{
	const char* serverName = "128.237.200.50";
	const char* scope = "DEFAULT_SCOPE";
	const char* port = "61616";
	vhmsg::g_vhmsg=(vhmsg::Client*)malloc(sizeof(vhmsg::Client));
	jlong vm=(jlong)vhmsg::g_vhmsg;
	LOGI("Address is ,%l",vm);
	//char *buf = (char*)malloc(10);
	//strcpy(buf, "1234567890");
	//jstring jstrBuf = env->NewStringUTF(buf);
//	*((jstring*)vm)=jstrBuf;
	//jlong vm=(jlong)malloc(sizeof(vhmsg::Client));
	//jobject global=env->NewGlobalRef(reinterpret_cast<jobject>(*vm));
	//vm.SetScope( scope );
	//bool a= vm.OpenConnection(serverName,port);

	LOGI("TTU Open  Success here,%l",vm);
	return vm;

}
#endif







   
    
   



#if 0
void closeConnection(JNIEnv* env, jobject thiz)
{
	LOGI("Close connection");
	//endConnection();
}

jint JNI_OnLoad(JavaVM* vm, void* reserved)
{
    LOG_FOOT;
    JNIEnv *env;

    LOGI("JNI_OnLoad called");
    if (vm->GetEnv((void**) &env, JNI_VERSION_1_4) != JNI_OK)
    {
    	LOGE("Failed to get the environment using GetEnv()");
        return -1;
    }

    JNINativeMethod methods[] =
    {
		{
		            "closeConnection",
		            "()V",
		            (void *) closeConnection
		},		
    };
    jclass k;
    k = (env)->FindClass ("com/android/sbjniapp/SBJNIAppLib");
    (env)->RegisterNatives(k, methods, 1);

    return JNI_VERSION_1_4;
}
#endif










