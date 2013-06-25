import java.util.*;

public class WhiteBoard extends Thread {

private BlockingQueue inQueue       = null;
private Hashtable     interestedParties = null;
private Hashtable     clients           = null;
private Hashtable     clientsReverse    = null;

private static WhiteBoard wb=null;
private boolean finished = false;

private WhiteBoard() {
	inQueue           = new BlockingQueue();
	interestedParties = new Hashtable();
	startThread();
}

public WhiteBoard getWhiteBoard() { 
	if (wb == null) wb = new WhiteBoard();
	return wb;
}

public void register(WhiteBoardClient wbc,String name) {
	clients.put(name,wbc);
	clientsReverse.put(wbc,name);
}

public void unregister(WhiteBoardClient wbc) {
	unregister(wbc,(String)clientsReverse.get(wbc));
}
public void unregister(String name) {
	unregister((WhiteBoardClient)clients.get(name),name);
}
public void unregister(WhiteBoardClient wbc,String name) {
	clients.remove(name);
	clientsReverse.remove(wbc);
}

public void inject(WhiteBoardClient wbc,Fact fact) {
	fact.setSource((String)clients.get(wbc));
	inQueue.add(fact);
}

public void endThread() { finished=true; }
public void startThread() { finished=false; this.start();}

public void run() {
	Fact fact=null;
	while(!finished) {
		fact= (Fact)inQueue.remove();
		recordFact(fact);
		if(fact.getDomain().compareTo(Fact.sCOMMAND)==0) {
			commandFact(fact);
		}
		triggerFact(fact);
	}
}

private void recordFact(Fact fact) {
	System.out.println("RECORD:\n\t"+fact.toString()); // heh, just dump it for now
}
private void triggerFact(Fact fact) {
	String id=null;
	Hashtable scores=new Hashtable();
	Enumeration e=null;
	String k; // key
	int    v; // val

	// for id in subject,verb,object
	// score interested parties
	for(int i=0;i<3;i++) {
		Hashtable interested=null;
		switch(i) {
			case 0: id=fact.getSubject(); break;
			case 1: id=fact.getVerb(); break;
			case 2: id=fact.getObject(); break;
			default: return;
		}
		interested=(Hashtable)interestedParties.get(id);
		if(interested!=null) {
			//add interested state machines to score list :-)
			for(e = interested.keys(); e.hasMoreElements();) {
				k=(String)e.nextElement();
				v=((Integer)interested.get(k)).intValue();
				Integer sc=(Integer)scores.get(k);
				if (sc==null) {
					scores.put(k, new Integer(v));
				} else {
					int scor=sc.intValue()+v;
					scores.put(k, new Integer(scor));
				}
			} // end interested state machines update score
		} //end iterested state machines
	}// end for
	//interested parties 
	//priority = number of matches
	// 1= one match 2= two matches 3= three matches
	// priorities can change to overflow 3, so just take >2
	// TODO: limit to most important?
	for(e = scores.keys(); e.hasMoreElements();) {
		k = (String)e.nextElement();
		v = ((Integer)scores.get(k)).intValue();
		if (v>2) {
			processStateMachine(k,fact);
		}
	}
}
private void commandFact(Fact fact) {
	// execute commands
	// subject = WhiteBoardClient
	// verb    = command
	// object  = parameters
	System.out.println("FACT:\n\t"+fact.toString());
}

private void processStateMachine(String stateMachineGUID,Fact f) {
}


} // end class
