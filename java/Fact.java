import java.util.*;

public class Fact {

private String source =null;
private String subject=null;
private String verb   =null;
private String object =null;
private float  value  =0;
private String domain =null;
private long   time   =0;
private String replicationid=null;

public  static final float  fNULL   =-1;
public  static final String sNULL   ="*NULL";
public  static final String sDEFAULT="*DEFAULT";
public  static final String sCOMMAND="*COMMAND";
public  static final String sUNKNOWN="*UNKNOWN";

public Fact(String subj, String verb, String obj,float val,String domain,long time) {
	init(subj,verb,obj,val,domain,time);
}
public Fact(String subj, String verb, String obj,float val,String domain) {
	init(subj,verb,obj,val,domain,0);
}
public Fact(String subj, String verb, String obj,float val) {
	init(subj,verb,obj,val,null,0);
}
public Fact(String subj, String verb, String obj,String domain) {
	init(subj,verb,obj,fNULL,domain,0);
}
public Fact(String subj, String verb, String obj) {
	init(subj,verb,obj,fNULL,null,0);
}
public Fact(Hashtable h) {
	this.replicationid = (String) h.get("ID");
	String subject = (String) h.get("SUBJECT");
	String verb    = (String) h.get("VERB");
	String object  = (String) h.get("OBJECT");
	String s_val   = (String) h.get("VALUE");
	String domain  = (String) h.get("DOMAIN");
	String source  = (String) h.get("SOURCE");
	String s_time  = (String) h.get("TIME");
	float  f_val =0;
	long   l_time=0;
	try {
		f_val=Float.valueOf(s_val).floatValue();
	} catch(Exception a) {}
	try {
		l_time=Long.valueOf(s_time).longValue();
	} catch(Exception b) {}
	init(subject,verb,object,f_val,domain,l_time);
	if(source!=null) this.source=source;
}
private void init(String subj,String verb,String obj,float val,
		String domain,long time) {
	source =sUNKNOWN;
	if (subj!=null)   this.subject=subj;
	else              this.subject=sNULL;
	if (verb!=null)   this.verb   =verb;
	else              this.verb   =sNULL;
	if (obj !=null)   this.object =object;
	else              this.object =sNULL;
	if (domain!=null) this.domain =domain;
	else              this.domain =sDEFAULT;
	this.value =val;
	if (time  !=0)    this.time   =time  ;
	else              this.time   =System.currentTimeMillis();
}

public String getSubject() { return this.subject; }
public String getVerb()    { return this.verb; }
public String getObject()  { return this.object; }
public float  getValue()   { return this.value; }
public String getDomain()  { return this.domain; }
public long   getTime()    { return this.time; }
public String getReplicationId() {
	if (replicationid == null) {
		String tmp=subject+":"+verb+":"+object;
		replicationid=MD5.getHashStringBase64(tmp);
	}
	return replicationid;
}
public String getSource() { return this.source; }
public void   setSource(String source) { this.source=source; }
public String toString() {
	StringBuffer sb=new StringBuffer();
	sb.append("<FACT ");
	sb.append("ID=\""     +getReplicationId()+"\" ");
	sb.append("SUBJECT=\""+getSubject()+"\" ");
	sb.append("VERB=\""   +getVerb()   +"\" ");
	sb.append("OBJECT=\"" +getObject() +"\" ");
	sb.append("VALUE=\""  +getValue()  +"\" ");
	sb.append("DOMAIN=\"" +getDomain() +"\" ");
	sb.append("SOURCE=\"" +getSource() +"\" ");
	sb.append("TIME=\""   +getTime()   +"\" ");
	sb.append("/>");
	return sb.toString();
}
}//end class
