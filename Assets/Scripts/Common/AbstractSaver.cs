using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class AbstractSaver<T> : IDisposable where T : new()
    {
	    public T Data => _data;
	    
	    private SemaphoreSlim _semaphore;
        private BinaryFormatter _formatter = new BinaryFormatter();
        private string _path;
        private T _data;
    
        public AbstractSaver(string name)
        {
            _path = Path.Combine(Application.persistentDataPath, name);
            _semaphore = new SemaphoreSlim(1, 1);
        }
    
        public async Task Save()
        {
	        await _semaphore.WaitAsync();
	        
	        await Task.Run(async () =>
	        {
		        try
		        {
			        using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate)) 
			        {
				        _formatter.Serialize(fs, _data);
			        }
		        }
		        finally
		        {
			        _semaphore.Release();
		        }
	        }).ConfigureAwait(false);
        }
    
        public async Task Load()
        {
	        try
	        {
		        _data = await Task.Run(() =>
		        {
			        using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate)) 
			        {
				        return (T)_formatter.Deserialize(fs);
			        }
		        }).ConfigureAwait(false);
	        }
	        catch (Exception e)
	        {
		        _data ??= new T();
		        await Save();
	        }
        }
    
        public void Dispose()
        {
        	Save().Wait();
        }
    }
}