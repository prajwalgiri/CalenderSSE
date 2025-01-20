
import { ToastContainer, toast } from 'react-toastify';
import { useEffect, useState } from 'react';
export default function NotificationContainer() { 
    const[data , UpdateData] = useState('data')
    useEffect(() => {
        // First, we need to create an instance of EventSource and pass the data stream URL as a
        const es = new EventSource("/sse");
        // parameter in its constructor
       // Whenever the connection is established between the server and the client we'll get notified
        es.onopen = () => console.log(">>> Connection opened!");
       // Made a mistake, or something bad happened on the server? We get notified here
        es.onerror = (e) => console.log("ERROR!", e);
       // This is where we get the messages. The event is an object and we're interested in its `data` property
        es.onmessage = (e) => {

            console.log(">>>", e);
            UpdateData(e.data);
            toast(e.data);
        };
       // Whenever we're done with the data stream we must close the connection
        return () => es.close();
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