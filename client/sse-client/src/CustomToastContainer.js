import { useState, useEffect } from 'react';
import { toast,ToastContainer } from 'react-toastify';


  const baseUrl= 'https://localhost:7183/';
  const username ='prajwal'+new Date().toTimeString();
 export default function CustomToast() {
    
    const [data, setData] = useState('--');
    const [id, setId] = useState();
   
useEffect(() => {
    const evtSource = new EventSource(baseUrl+'notifications?name='+username);
   
    evtSource.onmessage = (event) => {
      if (event.data) {
        setData(event.data);
        console.log(event,'event');
        setId(JSON.parse(event.data).id);
        toast(Msg(event.data));
        console.log(JSON.parse(event.data))
      }
    };
}, []);
const Msg = (msgdata) => (
    
    <div>
      <p>
        {msgdata}
        </p>
      <button onClick={handleclose(msgdata)}>Close</button>
    </div>
  );
  var handleclose= function(toastData){
    console.log(toastData,'toast data')
    let d= JSON.parse(toastData);
    console.log(d,'tostdata obj')
    fetch(baseUrl+'notifications/mark-as-read?id='+d.id+'&user='+username)
    .then(response=>{
        if(response.ok){
            toast("Sent as Read Id: "+d.id)
        }
    });
  }
return (
    <div>
        <p >{data}</p>
        <ToastContainer />
    </div>
);
  }