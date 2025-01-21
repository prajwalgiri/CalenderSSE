
import { ToastContainer, toast } from 'react-toastify';
import { useEffect,  useState } from 'react';
export default function NotificationContainer() { 

    

const [data, setData] = useState('data');
useEffect(() => {
    const evtSource = new EventSource('https://localhost:7183/sse');
    evtSource.onmessage = (event) => {
      if (event.data) {
        setData(event.data);
        toast(event.data);
      }
    };
}, []);


    const notify = () => toast("Wow so easy!");
    return (
        <div>
            <button onClick={notify}>Notify!</button>
            <p >{data}</p>
            <ToastContainer />
        </div>
    );
}