package com.yahoo.inmind.middleware.control;

import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.util.Log;

import com.yahoo.inmind.middleware.events.AudioRecordEvent;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Created by oscarr on 2/26/15.
 */
public class AudioController extends Thread{

    private boolean recording;
    private boolean stopped = false;

    private static AudioConfig currentRecorder = null;
    private MessageBroker mb;
    private static AudioController instance;
    private static Map<Object, AudioConfig> configurations;

    private AudioController( MessageBroker mb){
        this();
        this.mb = mb;
    }

    /**
     * Give the thread high priority so that it's not canceled unexpectedly, and start it
     */
    private AudioController(){
        android.os.Process.setThreadPriority(android.os.Process.THREAD_PRIORITY_URGENT_AUDIO);
        if( configurations == null ){
            configurations = Collections.synchronizedMap(new LinkedHashMap());
        }
    }

    /**
     * Singleton
     * @param sampleRateInHz
     * @param channelConfig
     * @param audioEncoding
     * @param mb
     * @param subscriber
     * @return
     */
    public static AudioController getInstance( Integer sampleRateInHz, Integer channelConfig, Integer audioEncoding, Integer bufferElements2Rec, Integer bytesPerElement, MessageBroker mb, Object subscriber ) {
        if (instance == null) {
            instance = new AudioController( mb );
        }
        // add the subscriber to the waiting list
        if( configurations.containsKey( subscriber ) == false ){
            AudioConfig config = new AudioConfig( sampleRateInHz, channelConfig, audioEncoding, bufferElements2Rec, bytesPerElement, subscriber );
            configurations.put( subscriber, config );
            if( currentRecorder == null ){
                currentRecorder = config;
            }
        }
        if( instance.isAlive() == false ){
            instance.start();
        }
        return instance;
    }

    /**
     * Singleton
     * @param mb
     * @return
     */
    public static AudioController getInstance( MessageBroker mb ) {
        if (instance == null) {
            instance = new AudioController( mb );
        }
        if( instance.isAlive() == false ){
            instance.start();
        }
        return instance;
    }


    @Override
    public void run(){

        try{
            /*
             * Loops until something outside of this thread stops it.
             * Reads the data from the currentRecorder and writes it to the audio track for playback.
             */
            while ( !stopped ) {
                initializeRecordObj();
                while( recording && currentRecorder != null ) {
                    currentRecorder.recordAndSend();
                }
            }
        } catch(Throwable x){
            Log.w("Audio", "Error reading voice audio", x);
        }
    }


    /**
     * Frees the thread's resources after the loop completes so that it can be run again
     */
    public void releaseController(){
        stopped = true;
        for( AudioConfig ac : configurations.values() ){
            ac.release();
        }
        configurations.clear();
        configurations = null;
        instance.interrupt();
        currentRecorder = null;
        instance = null;
    }

    /**
     * Initialize buffer to hold continuously recorded audio data, start recording, and start
     * playback.
     */
    private void initializeRecordObj() {
        try {
            currentRecorder.getRecorder().startRecording();
            recording = true;
        }catch(Exception e){
            currentRecorder.sendErrorMessage();
            unsubscribe( currentRecorder.getSubscriber() );
        }
    }


    //convert short to byte
    private byte[] short2byte( short[] sData ) {
        int shortArrsize = sData.length;
        byte[] bytes = new byte[shortArrsize * 2];
        for ( int i = 0; i < shortArrsize; i++ ) {
            bytes[i * 2] = (byte) (sData[i] & 0x00FF);
            bytes[(i * 2) + 1] = (byte) (sData[i] >> 8);
            sData[i] = 0;
        }
        return bytes;
    }

    /**
     * Called from outside of the thread in order to stop the recording/playback loop
     */
    public void unsubscribe( Object subscriber ){
        for( Object subs : configurations.keySet() ){
            if( subs == subscriber ){
                AudioConfig config = configurations.remove( subs );
                config.release();
                break;
            }
        }
        if( configurations != null && configurations.isEmpty() ) {
            releaseController();
        } else if( configurations != null && configurations.values().size() > 0 ){
            Object key = configurations.keySet().iterator().next(); //the next config in the
            currentRecorder = configurations.get(key);
            recording = false;
        }
    }


    /**
     * Helper class that contains each particular audio record configuration and the corresponding subscriber
     */
    static class AudioConfig{
        private AudioRecord recorder;
        private int bufferElements2Rec = 1024; // want to play 2048 (2K) since 2 bytes we use only 1024
        private int bytesPerElement = 2; // 2 bytes in 16bit format
        private int sampleRateInHz = 44100;
        private int channelConfig = AudioFormat.CHANNEL_IN_MONO;
        private int audioEncoding = AudioFormat.ENCODING_PCM_16BIT;
        private int sizeInBytes;
        private short[] buffer;
        private boolean flagRecord = true;
        private boolean isNewConfiguration = true;
        private Object subscriber;

        AudioConfig(Integer sampleRate, Integer channelConfig, Integer audioEncoding, Integer bufferElements2Rec, Integer bytesPerElement, Object subscriber) {
            this.subscriber = subscriber;
            if( sampleRate != null ) {
                this.sampleRateInHz = sampleRate;
            }
            if( channelConfig != null && (channelConfig == AudioFormat.CHANNEL_IN_STEREO || channelConfig == AudioFormat.CHANNEL_IN_MONO ) ){
                this.channelConfig = channelConfig;
            }
            if( audioEncoding != null && audioEncoding >= AudioFormat.ENCODING_DEFAULT && audioEncoding <= AudioFormat. ENCODING_E_AC3 ) {
                this.audioEncoding = audioEncoding;
            }
            if( bufferElements2Rec != null && bufferElements2Rec > AudioRecord.getMinBufferSize( this.sampleRateInHz, this.channelConfig, this.audioEncoding)  ){
                this.bufferElements2Rec = bufferElements2Rec;
            } else{
                this.bufferElements2Rec = AudioRecord.getMinBufferSize( this.sampleRateInHz, this.channelConfig, this.audioEncoding);
            }
            if( bytesPerElement != null && bytesPerElement > 1){
                this.bytesPerElement = bytesPerElement;
            }

            recorder = new AudioRecord( MediaRecorder.AudioSource.MIC,
                this.sampleRateInHz,
                this.channelConfig,
                this.audioEncoding,
                this.bufferElements2Rec * this.bytesPerElement);
            buffer = new short[bufferElements2Rec];
        }

        public AudioRecord getRecorder() {
            return recorder;
        }

        public synchronized void release(){
            flagRecord = false;
            currentRecorder.getRecorder().release();
            if( recorder != null ) {
                recorder.release();
                recorder = null;
            }
        }

        /**
         * Builds an AudioRecordEvent containing the read buffer and send it to all subscribers
         */
        public synchronized void recordAndSend() {
            if( flagRecord ) {
                AudioRecordEvent event = new AudioRecordEvent();
                sizeInBytes = recorder.read(buffer, 0, bufferElements2Rec);
                // writes the data to file from buffer. stores the voice buffer
                byte bData[] = AudioController.instance.short2byte(buffer);
                event.setBuffer(bData);
                event.setSizeInBytes(sizeInBytes);
                event.setMinBufferSize(bufferElements2Rec * bytesPerElement);
                event.setSampleRate(sampleRateInHz);
                event.setChannelConfig(channelConfig);
                event.setAudioEncoding(audioEncoding);
                if( isNewConfiguration ){
                    isNewConfiguration = false;
                    event.setNewConfiguration(true);
                }
                AudioController.instance.mb.send(event);
            }
        }

        public Object getSubscriber() {
            return subscriber;
        }

        public void sendErrorMessage() {
            AudioRecordEvent event = new AudioRecordEvent();
            event.setErrorMessage( "AudioRecord cannot be initialized with configuration:" +
                                    " Sample Rate: " + sampleRateInHz +
                                    " Channel: " + channelConfig +
                                    " Encoding: " + audioEncoding +
                                    " Buffer Elements: " + bufferElements2Rec +
                                    " Bytes per element: " + bytesPerElement);
            AudioController.instance.mb.send(event);
        }
    }
}
