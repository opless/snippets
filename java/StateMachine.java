import java.io.*;
import java.util.*;
import qdxml.*;

public class StateMachine implements DocHandler {
private String GUID       =null;
private String comment    =null;
private int    startstate =0;
private Hashtable States  =null; // (stateid) , (Vector [Fact] Conditions)
private Hashtable Execute =null; // (stateid) , (Vector [Fact] Execute)

private Stack  loadStack;
private StateMachine() {}

public static  StateMachine loadStateMachine(String filename) {
	StateMachine sm = new StateMachine();
	sm.load(filename);
	return sm;
}

private void load(String filename) {
	System.out.println("<!-- "+filename+" -->");
	loadStack = new Stack();
	try {
		QDParser.parse(this, new FileReader(filename)); 
	} catch (Exception e) {
		System.out.println("!!! "+e.toString());
		e.printStackTrace();
	}
}

public void startElement(String tag,Hashtable h) throws Exception {
	System.out.print("<"+tag);
	for(Enumeration e = h.keys(); e.hasMoreElements();) { 
		String k=(String) e.nextElement();
		String v=(String) h.get(k);
		System.out.print(" "+k+"=\""+v+"\"");
	}
	System.out.println(">");
	h.put("__TAG",tag);
	if (tag.compareTo("FACT")==0) System.out.println("::"+(new Fact(h)).toString());
	if (tag.compareTo("SMML")==0) {
		loadStack.push
	}
	if (tag.compareTo("SM")==0) {
		GUID   = h.get("GUID");
		comment= h.get("COMMENT");
	}
	if (tag.compareTo("STATE")==0) {  // id
	}
	if (tag.compareTo("EXECUTE")==0) {
	}
	if (tag.compareTo("CONDITIONS")==0) {
	}

}
public void endElement(String tag) throws Exception {
	System.out.println("</"+tag+">");
}
public void startDocument() throws Exception {
	System.out.println("<!-- start -->");
}
public void endDocument() throws Exception {
	System.out.println("<!-- end -->");
}
public void text(String str) throws Exception {
	System.out.print(str);
}

public static void main(String[] s) {
	loadStateMachine("1");
}
}//end class
