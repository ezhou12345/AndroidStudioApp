package com.yahoo.inmind.model.vo;


import com.yahoo.inmind.model.vo.ValueObject;

public class JsonItem extends ValueObject {
	private String json = null;

	public JsonItem()
	{
		json = null;
	}

	public String getRawString()
	{
        return json;
	}
}
