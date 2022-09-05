import styles from '../Style';
import { useState } from 'react';
import { Button } from 'react-bootstrap';
import { getAuth,fetchSignInMethodsForEmail,signInWithEmailAndPassword,updateProfile,createUserWithEmailAndPassword} from "firebase/auth";
import { UsersBaseUrl } from './UserControl';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
const EmailAndPassword = (props)=>{

  const [name,setName] =useState(props.user&&props.user.displayName? props.user.displayName:"");  
  const [email,setEmail] =useState("");  
  const [password,setPassword] =useState("");  
  const [confirmPassword,setConfirmPassword] =useState("");  
  const [verificationCode,setVerificationCode] =useState(""); 
  const [registerButtonString, setRegisterButtonString] = useState("Register");
  const [registrationClicked, setregistrationClicked] = useState(false);
  const [verificationCodeSent,setVerificationCodeSent]  = useState(false);
  

  const changeName = (event)=> {
    setName(event.target.value)
  }

  const changeEmail = (event)=> {

    setEmail(event.target.value);
    
  }

  const changePassword = (event)=> {
    setPassword(event.target.value);
  }
  
  const changeConfirmPassword = (event)=> {
    setConfirmPassword(event.target.value);
  }
  const changeVerificationCode = (event)=> {
    setVerificationCode(event.target.value);
  }




  const auth = getAuth();


  const handleLogin = async ()=>{
    if(password.length<6){
      alert("Wrong email or password");
      return
    }

    await fetchSignInMethodsForEmail(auth,email)
    .then((arr)=>{
      if(arr.length>0){
        // setMesage("")
        signInWithEmailAndPassword(auth, email, password)
        .then((userCredential) => {
          // Signed in 
          const user = userCredential.user;
         updateProfile(user,{
            displayName: name
          })

          // ...
        })
        .catch((error)=>{
          switch(error.code){
            case "auth/wrong-password":
              alert("Wrong email or password");
              break;
            
            case "auth/too-many-requests":
              alert("Let's refresh the page and try again");
              window.location.reload(false);
              break;
            
            case "auth/internal-error":
              alert ("Internal error , please refresh and try again")
              window.location.reload(false);
              break;
          }
      })
      }
      else{ //user never signed in
        alert("Wrong Email / You were signed with another method?")
      
      }
    });

  }

  const checkStateAndSendMail= () =>{
    if(!registrationClicked){
      setregistrationClicked (true)
      setRegisterButtonString("Verify")
      return; 
    }
    if(name.length===0){
      alert("Please enter your name");
      return
    }
    if(password.length<6){
      alert("Password need to be at least 6 chars");
      return
    }

    else if(password !== confirmPassword){
      alert("Passwords did not match")
      return
    }
         
    setVerificationCodeSent(true)
    let code = Math.floor(Math.random() * 1000000).toString()
    let formdata = new FormData();
    formdata.append("verificationCode",code)
    formdata.append("email",email)
    fetch(`${UsersBaseUrl}/sendVerificationMail`,
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
  .then((_) => {
    setVerificationCodeSent(true)
    setRegisterButtonString("Done")
    toast ("Verification Mail Sent :)");
  });
  }

  const register = ()=>{
    createUserWithEmailAndPassword(auth, email, password)
              .then((userCredential) => {
                // Signed in 
                const user = userCredential.user;
                updateProfile(user,{
                  displayName: name
                })
                let formdata = new FormData();

                formdata.append("id",userCredential.uid)
                formdata.append("displayName",name)
                formdata.append("email",email)
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
              })
              .catch(
                (error)=>{
                
                alert("Something went wrong.. check your email / password / verification code")
              
              })
  }
 
    return (
      
      <div  style={styles.newCoupon}>
        

        {
          registrationClicked &&
          <div>
            Name:
        <input type="text" value={name}   style={styles.formline} placeholder="Please enter your name" onChange={changeName} />
          </div>
        }
        Email:
        <input type="email" value={email} style={styles.formline} onChange={changeEmail} />
        
        Password
        <input type="password"  value={password} style={styles.formline} onChange={changePassword}  />

        {
          registrationClicked&&
          <div>
        
            Confirm Password
            <input type="password"  minLength={6} value={confirmPassword} style={styles.formline} onChange={changeConfirmPassword}  />
              </div>
        }
        {
          verificationCodeSent&&
          <div>
            Verification Code
          <input type="text" value={verificationCode} style={styles.formline} onChange={changeVerificationCode}  />
          </div>
        }
        
        <Button onClick={handleLogin} >Login</Button>

        <Button hidden={!verificationCodeSent}  onClick={register} >{registerButtonString}</Button> 
        <Button hidden={verificationCodeSent} id="registrationButton" onClick={checkStateAndSendMail} >{registerButtonString}</Button>

        <ToastContainer 
        position="top-center"
            autoClose={5000}
            hideProgressBar={true}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            pauseOnHover/>
      </div>
        
    );
}

























export default EmailAndPassword;

