import java.net.*;
import java.util.*;

public class GUID {

private int time    =0; // 32 bits of time.
private int node    =0; // 32 bits of randomness
private Random rand =null;
private String sysid=null;
private int iHash   =0;
private int addr    =0;

public GUID() {

	StringBuffer tmp= new StringBuffer();
	// sort out random seed :-)
	rand = new Random();

	// get InetAddress
	try { 
		InetAddress inet   = InetAddress.getLocalHost(); 
		byte[] bytes=inet.getAddress();
		addr = bytes[0] << 24;
		addr+= bytes[1] << 16;
		addr+= bytes[2] <<  8;
		addr+= bytes[3];
	} catch(Exception e) {
		// we couldn't determine ip address
		// so we fake one :P
		addr = (rand.nextInt() & 0xFF) << 24;
		addr+= (rand.nextInt() & 0xFF) << 16;
		addr+= (rand.nextInt() & 0xFF) <<  8;
		addr+= (rand.nextInt() & 0xFF) ;
	}

	
	String hexIp = toHex(addr);

	//get hashcode skew it a little just in case of multiple jvms
	iHash=System.identityHashCode(this) ^ rand.nextInt();
	String sHash = toHex(iHash); 



	//tmp.append(hexIp);
	//tmp.append('.');
	//tmp.append(sHash);// IPad
	//tmp.append('.');
	//sysid=tmp.toString();
	node = rand.nextInt();
}
public String getNextId() {
	//StringBuffer tmp=new StringBuffer(sysid);
	StringBuffer tmp=new StringBuffer();
	time = (int) (System.currentTimeMillis() & 0xFFFFFFFF);
	node = rand.nextInt();
	//tmp.append(toHex(time));
	//tmp.append('.');
	//tmp.append(toHex(node));

	byte[] string = new byte[16]; // 4 ints = 16 bytes
	string[ 0] =(byte) ((addr >> 24) & 0xFF);
	string[ 1] =(byte) ((addr >> 16) & 0xFF);
	string[ 2] =(byte) ((addr >>  8) & 0xFF);
	string[ 3] =(byte) ((addr      ) & 0xFF);

	string[ 4] =(byte) ((iHash>> 24) & 0xFF);
	string[ 5] =(byte) ((iHash>> 16) & 0xFF);
	string[ 6] =(byte) ((iHash>>  8) & 0xFF);
	string[ 7] =(byte) ((iHash     ) & 0xFF);

	string[ 8] =(byte) ((time >> 24) & 0xFF);
	string[ 9] =(byte) ((time >> 16) & 0xFF);
	string[10] =(byte) ((time >>  8) & 0xFF);
	string[11] =(byte) ((time      ) & 0xFF);

	string[12] =(byte) ((node >> 24) & 0xFF);
	string[13] =(byte) ((node >> 16) & 0xFF);
	string[14] =(byte) ((node >>  8) & 0xFF);
	string[15] =(byte) ((node      ) & 0xFF);

	tmp.append(Base64.encode(string));

	return tmp.toString().substring(0,22);
}

public String toHex(int integer) { return toHex(integer,8); }
public String toHex(int integer, int size) {
	String str = Integer.toHexString(integer);
	if (str.length()<size) {
	  StringBuffer sb= new StringBuffer();
	  for(int i = 0; i < (size-str.length()); i++) sb.append('0');
	  sb.append(str);
	  return sb.toString();
	} else {
	  return str;
	}
}
public static void main(String args[]) {
	GUID uid = new GUID();
	for(int i=0;i<16;i++) {
		System.out.println(uid.getNextId());
	}
}

}//end class
	
