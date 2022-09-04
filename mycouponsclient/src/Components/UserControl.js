import React, { useState , useContext } from "react";

import styles from "../Style";
import { initializeApp } from "firebase/app";
import { getAuth,GoogleAuthProvider , signInWithPopup , signOut ,onAuthStateChanged } from "firebase/auth";
import { Details } from "./Table/Table";
import { Button } from "react-bootstrap";
import Popup from "reactjs-popup";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {  } from '@fortawesome/fontawesome-free-solid'
import { faGoogle } from '@fortawesome/free-brands-svg-icons'
import EmailAndPassword from "./EmailAndPassword";

const firebaseConfig = {
  apiKey: "AIzaSyDOODrdL-2WkVa6nUfAfRyZ89y5TPGPDJo",
  authDomain: "mycoupons-6058d.firebaseapp.com",
  databaseURL: "https://mycoupons-6058d-default-rtdb.firebaseio.com",
  projectId: "mycoupons-6058d",
  storageBucket: "mycoupons-6058d.appspot.com",
  messagingSenderId: "938452106785",
  appId: "1:938452106785:web:051d5abb45aad2bc58ed60",
  measurementId: "G-BK2KZ7NS6W"
};

const app = initializeApp(firebaseConfig); 
export const auth = getAuth();

export const UsersBaseUrl = "https://mycouponsorganizer.azurewebsites.net/api/users";//https://localhost:7233/api/users";



const UserControl =(props)=>{
  const myDetails = useContext(Details);
  const [popupOpen,setPopupOpen] =useState(false);  
  const [finished,setFinished] =useState(false);  
  const [user,setUser] =useState({});  
  // const [emai,setUser] =useState(null);  

  
 
  
  const SignInGoogle = () => {
    auth.languageCode = 'en';
    const provider = new GoogleAuthProvider();
    provider.setCustomParameters({
      'login_hint': 'user@example.com',
      "prompt": 'select_account'
    });
    provider.addScope('profile');
	  provider.addScope('email');
	  provider.addScope('openid');
  
  signInWithPopup(auth, provider)
  .then((result) => {

    user.email=result.user.email
    setFinished(!finished)
    setPopupOpen(false)
    setUser(result.user)

    let formdata = new FormData();
    formdata.append("id",result.user.uid)
    formdata.append("displayName",result.user.displayName)
    formdata.append("email",result.user.email)
    fetch(`${UsersBaseUrl}/newuser`,
    {
      method: 'post',
      mode: 'cors',
      'Access-Control-Allow-Origin': true,
      headers: {
      'Accept': 'application/json, text/plain,multipart/form-formdata',
      'Authorization': 'Bearer ' + sessionStorage.tokenKey
      },
      body: formdata
      
  })

    // ...
  }).catch((error) => {
    // Handle Errors here.
    console.log(error)
  
    
  });

  
  }



  const SignOut =()=>{
  signOut(auth).then((res) => {
    // Sign-out successful.
  }).catch((error) => {
    // An error happened.
  });
}

onAuthStateChanged(auth,
  (user1) => {

    setUser(user1)

}   
);

    return (
      <Details.Consumer>
      {(myDetails)=> 
      (
        <div>
          
        <Popup 
        trigger= {<FontAwesomeIcon icon="fa-solid fa-user" />}
        on='click'
        open={popupOpen}
        onOpen={()=>setPopupOpen(true)}
        position="bottom left"
        style={styles.editCoupon} 
          
        >

        <div style={styles.UserControl}>

          {/* //details */}
          {user? 
          `Connected ${user.email}`
          : ""}
          
          {/* Sign in / Sign out */}
          {user? 
          
          <Button onClick={SignOut}>Sign out</Button>
          :
          <div>
            Let's Sign In :)

                <EmailAndPassword />
                Or With: 
                <FontAwesomeIcon style={styles.icon} onClick={SignInGoogle} icon={faGoogle} />
          </div>
        }

        </div>
        </Popup>
        Hello {user !== null ? user.displayName : "" }
      
    </div>

      )}
      </Details.Consumer>
      
    );
}

export default UserControl;