import java.util.*;

public class BlockingQueue
{
	private  Vector vecQ = null;
	private static final int MAX_Q_SIZE = 5000;
	
	public BlockingQueue()
	{ 
		vecQ = new Vector(500,500);
	}
	
	
	public synchronized void add(Object obj)
	{
		if (size() <= MAX_Q_SIZE)
		{
			vecQ.addElement(obj);
			notify();
		}
	}
	
	
	public synchronized Object remove()
	{
		while(vecQ.isEmpty())
		{
			try
			{
				wait();
			}
			catch(Exception e){}
		}

		Object ret = vecQ.elementAt(0);
		vecQ.removeElementAt(0);
		return ret;
	}

	public int size()
	{
		return vecQ.size();
	}
}

