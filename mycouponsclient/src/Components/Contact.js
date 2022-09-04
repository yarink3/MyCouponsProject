import styles from '../Style';
import { useContext, useState } from 'react';
import { Details, GlobalUser } from './Table/Table';
import { Navigate } from "react-router-dom";
import Popup from 'reactjs-popup';
import { Button } from 'react-bootstrap';
import { userValidateDetails } from '../App';
import ReactLoading from 'react-loading';
import {  UsersBaseUrl } from './UserControl';



const Contact = (props)=>{
  let formdata = new FormData();
  const [freeText,setFreeText] =useState("");  
  const [popupOpen,setPopupOpen] =useState(false);  
  const [isLoading,setIsLoading] = useState(false);
  const [finished,setFinished] =useState(false);  
  
  const changeFreeText = (event)=> {
    setFreeText(event.target.value)
  }

  const handleSubmit =  (event) => {
    setIsLoading(true)
    event.preventDefault();
    
    if(
      !userValidateDetails()

    ){
      return;
    }

    formdata.append("id",GlobalUser.user.uid);
    formdata.append("displayName",GlobalUser.user.displayName);
    formdata.append("email",GlobalUser.user.email);
    formdata.append("emailText",freeText);
    
    
    
    fetch(`${UsersBaseUrl}/support`
    , {
      method: 'post',
      mode: 'cors',
      'Access-Control-Allow-Origin': true,
      headers: {
      'Accept': 'application/json, text/plain,multipart/form-formdata',
      'Authorization': 'Bearer ' + sessionStorage.tokenKey
      },
      body: formdata
      
  })
  .then(_=>{
    setIsLoading(false)
    setPopupOpen(true);
  });

  }

    return (
      <Details.Consumer>
        {(myDetails)=>
        

      <div>
        <Popup 
        on='click'
        open={popupOpen}
        onOpen={()=>setPopupOpen(true)}
        onClose={()=>setFinished(true)}
        >
          <div  style={styles.popup}>
            נשלח מייל לתמיכה, תקבל תשובה בהקדם :)
          </div> 
          <div>
            <Button style={styles.button} onClick={()=>setFinished(true)}  >סגור</Button>
          
          </div>
        </Popup>

        {finished && (
          <Navigate to="/" replace={true} />
        )}

      {(!isLoading)&&
      <form onSubmit={handleSubmit} style={styles.newCoupon}>
        Please add your text here:
        <div>
        <textarea type="text"  rows="10" cols="80" value={freeText} maxLength={1000}   onChange={changeFreeText} />
        </div>
        <div style={styles.submit}>
        <input type="submit" value="Send"  />

            
        </div>
       
      </form>
      }
      {
        isLoading &&
          <div style={styles.loading}>
            <ReactLoading type="spin" color="#123456" height={100} width={100} />
          </div>
      }
        
    </div>
      }
    </Details.Consumer>
      
    );
  // }
}

export default Contact;

