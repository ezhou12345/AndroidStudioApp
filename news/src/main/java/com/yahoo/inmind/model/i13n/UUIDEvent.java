package com.yahoo.inmind.model.i13n;

import java.util.Date;

import com.yahoo.inmind.model.vo.ValueObject;

public class UUIDEvent extends ValueObject {
	public String userid;
	public String uuids;
	public Date time;

	public UUIDEvent(String currentUserName, String uuidStr) {
		userid = currentUserName;
		uuids = uuidStr;
		this.time = new Date();
	}	
}
