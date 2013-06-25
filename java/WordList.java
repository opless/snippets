import java.io.*;

import java.net.*;
import java.util.*;

public class WordList {
Soundex soundex=null;
Hashtable cache=null;
Hashtable cacheTS=null;

private static final int CACHE_MAXSIZE=128;
private static final int CACHE_MINSIZE=100;
private static final String storeRoot = "DATASTORE";
private static WordList wl=null;
//////////////////////
public static WordList getWordList() {
	if (wl == null) wl = new WordList();
	return wl;
}
//////////////////////
private WordList() {
soundex = new Soundex();
}
//////////////////////
public String ids2words(String ids) {
StringTokenizer st=new StringTokenizer(ids);
StringBuffer words=new StringBuffer();

while(st.hasMoreTokens()) {
  String w=id2word(st.nextToken());
  if (w.compareTo("")==0) w=" ";
  words.append(w);
  }
return words.toString();
}
//////////////////////
public String words2ids(String words) {
StringBuffer sb=new StringBuffer(words);
StringBuffer id=new StringBuffer();
StringBuffer  w=null;

for(int i=0;i<sb.length();i++) {
  char c=sb.charAt(i) ;
  if ((c >= 'a' && c <='z' ) || (c >= 'A' && c <= 'Z')) {
    if (w==null) w=new StringBuffer();
    w.append(c);
    }
  else {
    if (w != null) {
      id.append(" ");
      id.append( word2id(w.toString()) );
      w=null;
      }
    if (c < ' ' ) c=' ';
    id.append(" ");
    id.append( word2id(""+c) );
    }
  }
if (w != null) {
  id.append(" ");
  id.append( word2id(w.toString()) );
  w=null;
  }

return id.toString();
}
//////////////////////

public String word2id(String word) {
String         sx = soundex.encode(word);
Properties bucket = getBucket(sx);
String         id = bucket.getProperty(word);
if (id == null) id= generateId(bucket,word);

return sx+id;
}


//////////////////////
public String id2word(String id) {
String         sx = parseSoundex(id);  // J104.5 = J104
String         si = parseSoundexId(id);// J104.5 = .5
Properties bucket = getBucket(sx);

String word = bucket.getProperty(si,"{unknown id:"+si+"}");

return word;
}

//////////////////////
/* obviously one should cache this, but I cant be bothered at 3am :-) */
private Properties getBucket(String bucketName) {
Properties p=new Properties();

if (cache == null) {
  // no-cache
  //System.out.println("getBucket:Cache NOT initialized - initializing");
  cache = new Hashtable();
  cacheTS = new Hashtable();
  //possibly preload some words etc here?
  }
//check cache

p = (Properties) cache.get(bucketName);
if (p==null) {
  // not in cache, load it.
  p=new Properties();
  try {
    FileInputStream fis=new FileInputStream(getFilename(bucketName));
    p.load(fis);
    fis.close();
    }
  catch(Exception e) { 
    p=new Properties();
    //System.out.println("Couldnt load bucket:"+bucketName+" in cache:"+cacheTS.size()+"::"+e); 
    //e.printStackTrace();
    }
  cache.put(bucketName,p); // slap it in the cache.
  }
cacheTS.put(bucketName,new Date());  
//now check the cache size I'd say 128 entries would be enough
//we check cacheTS cos its the size of its objects is smaller :-)


if (cacheTS.size() > CACHE_MAXSIZE) {
//System.out.println("*** Cache Size="+cacheTS.size());

  //System.out.println("*** flushing old entries ***");
  long state= 3600000; // 3600x1000 = 1 hour

  while(cacheTS.size()>CACHE_MINSIZE) {
    Date d=new Date(System.currentTimeMillis()-state);

    
    //System.out.print("*** Purging: ");
    for(Enumeration e= cacheTS.keys();e.hasMoreElements();) {
      Object k=e.nextElement();
      if (d.after( (Date) cacheTS.get(k) )) {
         //System.out.print(" "+k);
         cacheTS.remove(k);
         cache.remove(k);
         }
      }
    //System.out.println("\n*** Flushed, Current Size="+cacheTS.size()+" "+(new Date(state)));
    
    state=state>>1;
    System.gc();
    }
  }
return p;
}
//////////////////////
private String generateId(Properties bucket, String word) {
String bucketName = soundex.encode(word);
String id="0";
long    c=0;

try {
  c=Long.parseLong(bucket.getProperty("_MAX"));
  } catch(Exception ee) { 
    c=0; 
  }
try {
  c++;
  bucket.put("_MAX",""+c);
  id="."+c;
  bucket.put(id,word);
  bucket.put(word,id);
  FileOutputStream fos=new FileOutputStream(getFilename(bucketName));
  bucket.save(fos,"saved "+(new Date()).toString()+" max="+c);
  fos.close();
  }
catch(Exception e) { System.out.println("Couldnt save bucket:"+e); }
return id;
}
//////////////////////
private String parseSoundex(String s) { 
if (s==null) return null;
return s.substring(0,s.indexOf("."));
}
//////////////////////
private String parseSoundexId(String s) { 
if (s==null) return null;
return s.substring(s.indexOf("."));

}
//////////////////////
private int cacheSize() {
  if (cacheTS==null) return 0;
  return cacheTS.size();
}
//////////////////////
public String getFilename(String name) {
String dirname=storeRoot+File.separator+"words";
int i=0;
for(i=0;i<name.length()-1;i++) {
  dirname+=File.separator+name.charAt(i);
  }
File f=new File(dirname);
f.mkdirs();
//System.out.println("Filepath= ["+dirname+"] FILE OBJ=["+f.toString()+"]");
dirname+=File.separator+name.charAt(i);
String bname=dirname+".ht";
//System.out.println("File name for bucket ["+name+"] is ["+bname+"] cache="+cacheSize());
return bname;

}
//////////////////////
}// endclass
