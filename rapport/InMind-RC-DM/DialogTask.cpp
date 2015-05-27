 //=============================================================================
//
//   Copyright (c) 2000-2007, Carnegie Mellon University.
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//   are met:
//
//   1. Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in
//      the documentation and/or other materials provided with the
//      distribution.
//
//   This work was supported in part by funding from the Defense Advanced
//   Research Projects Agency and the National Science Foundation of the
//   United States of America, and the CMU Sphinx Speech Consortium.
//
//   THIS SOFTWARE IS PROVIDED BY CARNEGIE MELLON UNIVERSITY ``AS IS'' AND
//   ANY EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
//   PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL CARNEGIE MELLON UNIVERSITY
//   NOR ITS EMPLOYEES BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//   SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//   LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
//   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//=============================================================================


/*

Author: Alexandros Papangelis, 2015, apapa@cs.cmu.edu

Dialogue manager for the CMU-InMind Rapport project.

*/

//Debug build requires a Debug Build of Olympus

#include "DialogTask.h"
#include "DateUtil.h"
#include "StringUtil.h"
#include "BindingFilters.h"
	 
#define __IN_DIALOGTASK__

//-----------------------------------------------------------------------------
//
// DM GALAXY SERVER CONFIGURATION
//
//-----------------------------------------------------------------------------
#ifdef GALAXY
    DMSERVER_CONFIGURATION("DialogManager", 17000)
#endif

//-----------------------------------------------------------------------------
//
// CONSTANT DEFINITIONS
//
//-----------------------------------------------------------------------------

#include "Constants.h"

//-----------------------------------------------------------------------------
//
// CUSTOM MACROS
//
//-----------------------------------------------------------------------------

#define REQUEST_CONCEPT_AFTER_CLEAR(x)  \
    ON_INITIALIZATION(                  \
        C(#x).Clear();                  \
    )                                   \
    REQUEST_CONCEPT(x)

#define DATE_NUM(x) (date2num((int)C(#x".year"), (int)C(#x".month"), (int)C(#x".day")))
#define TIME_NUM(x) ((int)C(#x".hour") * 100 + (int)C(#x".minute"))
#define DATE_TIME_NUM(x) ((long long)DATE_NUM(x) + TIME_NUM(x))

//-----------------------------------------------------------------------------
//
// DIALOG CORE CONFIGURATION
//
//-----------------------------------------------------------------------------

CORE_CONFIGURATION(

    // declare the NLG and the GUI as output devices
    USE_OUTPUT_DEVICES(
        DEFAULT_OUTPUT_DEVICE("nlg", "nlg.launch_query", 1)
    )

    USE_BINDING_FILTERS(
        BINDING_FILTER("DateTime", DateTimeBindingFilter)
        BINDING_FILTER("Location", LocationBindingFilter)
        BINDING_FILTER("DoorNumber", DoorNumberBindingFilter)
    )
)

//-----------------------------------------------------------------------------
//
// CONCEPT TYPES DEFINITION
//
//-----------------------------------------------------------------------------

// RapportState struct
DEFINE_STRUCT_CONCEPT_TYPE( CRapportStateConcept,
	ITEMS(
		INT_ITEM(traitRapport)
		INT_ITEM(stateRapport)
	)
)

// DateTime struct
DEFINE_STRUCT_CONCEPT_TYPE( CDateTimeConcept,
    ITEMS(
        INT_ITEM(year)
        INT_ITEM(month)
        INT_ITEM(day)
        INT_ITEM(hour)
        INT_ITEM(minute)
        INT_ITEM(period_of_day)
        INT_ITEM(date_status)
        INT_ITEM(time_status)
    )
)

// Location Construct
DEFINE_STRUCT_CONCEPT_TYPE( CLocationConcept,
    ITEMS(
        STRING_ITEM(building)
        //STRING_ITEM(door_number)
        //INT_ITEM(digits)     // Number of digits of door number of this building
    )
)


// Person Construct
DEFINE_STRUCT_CONCEPT_TYPE( CPersonConcept,
    ITEMS(
        STRING_ITEM(name)
    )
)




// Location Result frame
DEFINE_FRAME_CONCEPT_TYPE( CLocation_resultConcept,
    ITEMS(
    	STRING_ITEM(building)
        STRING_ITEM(path)
        
    )
)


// Person Result frame
DEFINE_FRAME_CONCEPT_TYPE( CPerson_resultConcept,
    ITEMS(
        STRING_ITEM(name)
        STRING_ITEM(office_location)
        
    )
)



//-----------------------------------------------------------------------------
//
// AGENT SPECIFICATIONS
//
//-----------------------------------------------------------------------------

// /YahooNews
DEFINE_AGENCY( CYahooNews,
    IS_MAIN_TOPIC()
    DEFINE_SUBAGENTS(
        SUBAGENT(GreetWelcome, CGreetWelcome, "")
        SUBAGENT(NewsTask, CNewsTask, "")
		SUBAGENT(GreetGoodbye, CGreetGoodbye, "")
    )
)

// /YahooNews/GreetWelcome
 DEFINE_INFORM_AGENT( CGreetWelcome,
     PROMPT(":non-listening inform welcome")
 )


// /YahooNews/NewsTask
DEFINE_AGENCY( CNewsTask,
    DEFINE_CONCEPTS(
        INT_USER_CONCEPT(task_type, "")
        // For global communication when validating dates and times
        STRING_SYSTEM_CONCEPT(name_of_dt_to_validate)
        STRING_SYSTEM_CONCEPT(desc_of_dt_to_validate)
        CUSTOM_SYSTEM_CONCEPT(first_dt, CDateTimeConcept)
        CUSTOM_SYSTEM_CONCEPT(other_dt, CDateTimeConcept)
    )
	
	ON_INITIALIZATION(

	C("task_type").Clear();
	)
    DEFINE_SUBAGENTS(
        SUBAGENT(InitialQuery, CInitialQuery, "")
        SUBAGENT(PerformPlaceQuery, CPerformPlaceQuery, "")
        SUBAGENT(PerformPersonQuery, CPerformPersonQuery, "")
        SUBAGENT(RequestTaskTypeAgain, CRequestTaskTypeAgain, "")
		SUBAGENT(FillerText, CFillerText, "")
    )
    SUCCEEDS_WHEN(COMPLETED(PerformPlaceQuery) || (COMPLETED(PerformPersonQuery) || COMPLETED(RequestTaskTypeAgain)) || (int)C("task_type") == TASK_GOODBYE)
)


//-----------------------------------------------------------------------------
//
// YAHOO NEWS TASK
//
//-----------------------------------------------------------------------------


// /YahooNews/NewsTask/AskTopic
DEFINE_INFORM_AGENT(CAskTopic,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/NewsTask/SuggestTopic
DEFINE_INFORM_AGENT(CSuggestTopic,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/NewsTask/SuggestNewsArticle
DEFINE_INFORM_AGENT(CSuggestNewsArticle,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/NewsTask/PresentNews
DEFINE_INFORM_AGENT(CPresentNews,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/NewsTask/AskFeedback
DEFINE_INFORM_AGENT(CAskFeedback,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/NewsTask/FillerText
DEFINE_INFORM_AGENT(CFillerText,
	PROMPT(":non-listening inform goodbye")
)


//-----------------------------------------------------------------------------
//
// SOCIAL TASK
//
//-----------------------------------------------------------------------------

// /YahooNews/RapportTask
DEFINE_AGENCY(CRapportTask,
	DEFINE_CONCEPTS(
		INT_USER_CONCEPT(task_type, "")
		// For global communication when validating dates and times
		STRING_SYSTEM_CONCEPT(name_of_dt_to_validate)
		STRING_SYSTEM_CONCEPT(desc_of_dt_to_validate)
		CUSTOM_SYSTEM_CONCEPT(first_dt, CDateTimeConcept)
		CUSTOM_SYSTEM_CONCEPT(other_dt, CDateTimeConcept)
	)

	ON_INITIALIZATION(
		C("task_type").Clear();
	)
		DEFINE_SUBAGENTS(
		SUBAGENT(InitialQuery, CInitialQuery, "")
		SUBAGENT(PerformPlaceQuery, CPerformPlaceQuery, "")
		SUBAGENT(PerformPersonQuery, CPerformPersonQuery, "")
		SUBAGENT(RequestTaskTypeAgain, CRequestTaskTypeAgain, "")
		)
		SUCCEEDS_WHEN(COMPLETED(PerformPlaceQuery) || (COMPLETED(PerformPersonQuery) || COMPLETED(RequestTaskTypeAgain)) || (int)C("task_type") == TASK_GOODBYE)
)

// /YahooNews/RapportTask/QESelfDisclosure
DEFINE_INFORM_AGENT(CQESelfDisclosure,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/RapportTask/PositiveSelfDisclosure
DEFINE_INFORM_AGENT(CPositiveSelfDisclosure,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/RapportTask/NegativeSelfDisclosure
DEFINE_INFORM_AGENT(CNegativeSelfDisclosure,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/RapportTask/Praise
DEFINE_INFORM_AGENT(CPraise,
	PROMPT(":non-listening inform welcome")
)

// /YahooNews/RapportTask/Acknowledge
DEFINE_INFORM_AGENT(CAcknowledge,
	PROMPT(":non-listening inform welcome")
)



// /YahooNews/GreetGoodbye
DEFINE_INFORM_AGENT(CGreetGoodbye,
	PROMPT(":non-listening inform goodbye")
)

/*



//-----------------------------------------------------------------------------
//
// INITIAL QUERY START
//
//-----------------------------------------------------------------------------


///Calendar/NewsTask/InitialQuery
DEFINE_REQUEST_AGENT( CInitialQuery,
    
    //PROMPT(":non-listening inform welcome")
    //INPUT_LINE_CONFIGURATION("set_lm=TaskType")
    //PRECONDITION(!AVAILABLE(task_type))
    //PROMPT(":non-listening request task_type")
	
	PROMPT(":non-listening request task_type")
    REQUEST_CONCEPT(task_type)      // Mustn't use REQUEST_CONCEPT_AFTER_CLEAR because the precondition depends on the concept
    GRAMMAR_MAPPING(mysprintf("![FindPerson]>%d, ![FindPlace]>%d , ![Goodbye]>%d",
                              TASK_FIND_PERSON, TASK_FIND_PLACE, TASK_GOODBYE))
    SUCCEEDS_WHEN(AVAILABLE(task_type))
    )





//-----------------------------------------------------------------------------
//
// PLACE QUERY START
//
//-----------------------------------------------------------------------------


///Calendar/NewsTask/PerformPlaceQuery
DEFINE_AGENCY( CPerformPlaceQuery,
  PRECONDITION((int)C("task_type") == TASK_FIND_PLACE)
    DEFINE_CONCEPTS(
        //INT_USER_CONCEPT(query_type, "")
        CUSTOM_SYSTEM_CONCEPT(location, CLocationConcept)
        CUSTOM_SYSTEM_CONCEPT(location_result, CLocation_resultConcept)
    )
    ON_INITIALIZATION(
        C("location").Clear();
        C("location_result").Clear();
    )
    DEFINE_SUBAGENTS(
        SUBAGENT(RequestPlaceQuery, CRequestPlaceQuery, "")
        //SUBAGENT(HelpPlaceQuery, CHelpPlaceQuery, "")
        SUBAGENT(ExecutePlaceQuery, CExecutePlaceQuery, "")
        SUBAGENT(InformPlaceQueryDone, CInformPlaceQueryDone, "")
    )
    SUCCEEDS_WHEN(
        COMPLETED(InformPlaceQueryDone)
    )
)



// /Calendar/NewsTask/PerformPlaceQuery/RequestPlaceQuery
DEFINE_REQUEST_AGENT( CRequestPlaceQuery,
    PRECONDITION(UPDATED(task_type))        //intuitively useless. remove if there is an issue.
    //INPUT_LINE_CONFIGURATION("set_lm=LocationQuery")
    PROMPT(":non-listening request locationQuery")
    REQUEST_CONCEPT(location.building)
    GRAMMAR_MAPPING("![Location]")
    
    //GRAMMAR_MAPPING(mysprintf("![QueryExpression.Where]>%d, ![QueryExpression.How]>%d, "
    //                          "![QueryExpression.Which]>%d "
    //                          "![Help]>0",
    //                          QUERY_WHERE, QUERY_HOW, QUERY_WHICH))
    
    //MAX_ATTEMPTS(3)     // Fall into help if input can't be parsed
    
    //ON_COMPLETION(
    //    if (!AVAILABLE(query_type)) C("event").Clear();
    //)
)




// /Calendar/NewsTask/PerformPlaceQuery/HelpPlaceQuery
DEFINE_INFORM_AGENT( CHelpPlaceQuery,
    //PRECONDITION(!AVAILABLE(task_type))
    //PRECONDITION(!AVAILABLE(query_type) || (int)C("query_type") == 0)
    PROMPT(":non-listening inform help_locationQuery")
    ON_COMPLETION(
        C("task_type").Clear();
        A("../RequestPlaceQuery").ReOpenTopic();
        ReOpenTopic();
    )
)






// /Calendar/NewsTask/PerformPlaceQuery/ExecutePlaceQuery
DEFINE_EXECUTE_AGENT( CExecutePlaceQuery,
    PRECONDITION(AVAILABLE(location.building))
    //PROMPT("inform test")
    EXECUTE(pTrafficManager->Call(this, "gal_be.launch_query <task_type <location.building >location_result");)
)

// /Calendar/NewsTask/PerformPlaceQuery/InformPlaceQueryDone
DEFINE_INFORM_AGENT( CInformPlaceQueryDone,
	//PRECONDITION(AVAILABLE(event.type))
    PROMPT(":non-listening inform place_query <location_result")
)


//-----------------------------------------------------------------------------
//
// PERSON QUERY START
//
//-----------------------------------------------------------------------------


///Calendar/NewsTask/PerformPersonQuery
DEFINE_AGENCY( CPerformPersonQuery,
    PRECONDITION((int)C("task_type") == TASK_FIND_PERSON)
    DEFINE_CONCEPTS(
        CUSTOM_SYSTEM_CONCEPT(person, CPersonConcept)
        CUSTOM_SYSTEM_CONCEPT(person_result, CPerson_resultConcept)
        STRING_SYSTEM_CONCEPT(personResult)
    )
    ON_INITIALIZATION(
        C("person").Clear();
        C("person_result").Clear();
        C("personResult").Clear();
    )
    DEFINE_SUBAGENTS(
        SUBAGENT(RequestPersonQuery, CRequestPersonQuery, "")
        //SUBAGENT(HelpPersonQuery, CHelpPersonQuery, "")
        SUBAGENT(ExecutePersonQuery, CExecutePersonQuery, "")
        SUBAGENT(InformPersonQueryDone, CInformPersonQueryDone, "")
    )
    SUCCEEDS_WHEN(
        COMPLETED(InformPersonQueryDone)
    )
)



// /Calendar/NewsTask/PerformPersonQuery/RequestPersonQuery
DEFINE_REQUEST_AGENT( CRequestPersonQuery,
    PRECONDITION(UPDATED(task_type))
    //lmsesh INPUT_LINE_CONFIGURATION("set_lm=PersonQuery")
    //PROMPT(":non-listening request personQuery")
    PROMPT(":non-listening request personQuery")
    REQUEST_CONCEPT(person.name)
    GRAMMAR_MAPPING("![Person]")
    
    //GRAMMAR_MAPPING(mysprintf("![QueryExpression.Where]>%d, ![QueryExpression.How]>%d, "
    //                          "![QueryExpression.Which]>%d "
    //                          "![Help]>0",
    //                          QUERY_WHERE, QUERY_HOW, QUERY_WHICH))
    
    //MAX_ATTEMPTS(3)     // Fall into help if input can't be parsed
    
    //ON_COMPLETION(
    //    if (!AVAILABLE(query_type)) C("event").Clear();
    //)
)




// /Calendar/NewsTask/PerformPersonQuery/HelpPersonQuery
DEFINE_INFORM_AGENT( CHelpPersonQuery,
    PRECONDITION(!UPDATED(task_type))
    PROMPT(":non-listening inform help_personQuery")
    ON_COMPLETION(
        C("task_type").Clear();
        A("../RequestPersonQuery").ReOpenTopic();
        ReOpenTopic();
    )
)



// /Calendar/NewsTask/PerformPersonQuery/ExecutePersonQuery
DEFINE_EXECUTE_AGENT( CExecutePersonQuery,
    PRECONDITION(AVAILABLE(person.name))
    EXECUTE(pTrafficManager->Call(this, "gal_be.launch_query <task_type <person.name >person_result");)
    //EXECUTE(pTrafficManager->Call(this, "gal_be.launch_query <task_type <person.name >personResult");)
	)

// /Calendar/NewsTask/PerformPersonQuery/InformPersonQueryDone
DEFINE_INFORM_AGENT( CInformPersonQueryDone,
     //PRECONDITION(AVAILABLE(person.name) && AVAILABLE(person_result))
    //PRECONDITION(UPDATED(personResult))
    PROMPT(":non-listening inform person_query <person_result")
    //PROMPT("inform query_done < personResult")
    //PROMPT("inform query_done ")
	)




// /Calendar/NewsTask/RequestTaskTypeAgain
DEFINE_REQUEST_AGENT( CRequestTaskTypeAgain,
    //INPUT_LINE_CONFIGURATION("set_lm=TaskType")
    DEFINE_CONCEPTS(
        INT_USER_CONCEPT(task_type_again, "")
    )
    PRECONDITION((int)C("task_type") != TASK_GOODBYE)
    PROMPT(":non-listening request task_type_again")
    REQUEST_CONCEPT_AFTER_CLEAR(task_type_again)
    GRAMMAR_MAPPING("![Create]>1, ![Query]>2, ![Modify]>3, ![Delete]>4, ![Goodbye]>0")

    ON_COMPLETION(
        C("task_type") = (int)C("task_type_again");
        if ((int)C("task_type") != TASK_GOODBYE) {
            A("..").ReOpenTopic();
        }
    )
)*/


//-----------------------------------------------------------------------------
//
// AGENTS SHARED BY MULTIPLE AGENCIES
//
//-----------------------------------------------------------------------------




//-----------------------------------------------------------------------------
//
// AGENT DECLARATIONS
//
//-----------------------------------------------------------------------------
DECLARE_AGENTS(
	DECLARE_AGENT(CYahooNews)
        DECLARE_AGENT(CGreetWelcome)
        DECLARE_AGENT(CNewsTask)
            DECLARE_AGENT(CAskTopic)
            DECLARE_AGENT(CSuggestTopic)
			DECLARE_AGENT(CSuggestNewsArticle)
			DECLARE_AGENT(CPresentNews)
			DECLARE_AGENT(CAskFeedback)
			DECLARE_AGENT(CFillerText)
		DECLARE_AGENT(CRapportTask)
			DECLARE_AGENT(CQESelfDisclosure)
			DECLARE_AGENT(CPositiveSelfDisclosure)
			DECLARE_AGENT(CNegativeSelfDisclosure)
			DECLARE_AGENT(CPraise)
			DECLARE_AGENT(CAcknowledge)
		DECLARE_AGENT(CGreetGoodbye)
)

//-----------------------------------------------------------------------------
// DIALOG TASK ROOT DECLARATION
//-----------------------------------------------------------------------------
DECLARE_DIALOG_TASK_ROOT(YahooNews, CYahooNews, "")
