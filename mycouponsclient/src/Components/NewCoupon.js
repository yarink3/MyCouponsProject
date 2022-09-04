import styles from '../Style';
import { useContext, useState } from 'react';
import { Details, GlobalUser } from './Table/Table';
import { Navigate } from "react-router-dom";
import Popup from 'reactjs-popup';
import { Button } from 'react-bootstrap';
import { CouponsBaseUrl, getTodayString, userValidateDetails } from '../App';
import ReactLoading from 'react-loading';



const NewCoupon = (props)=>{
  const myDetails = useContext(Details)
  let formdata = new FormData();
  const [company,setCompany] =useState("");  
  const [ammount,setAmmount] =useState(0);  
  const [expireDateStr,setExpireDateStr] =useState("");  
  const [serialNumber,setSerialNumber] =useState("");  
  const [Image,setImage] =useState(null);  
  const [imageUrl,setImageUrl] =useState("");  
  const [finished,setFinished] =useState(false);  
  const [popupOpen,setPopupOpen] =useState(false);  
  const [isLoading,setIsLoading] = useState(false);

  const changeImage = (event) => {
    // set img in the form so the client can check his upload.
    // change the selected image in the coupon to be sent to the server.

    event.preventDefault();
    for (var index = 0; index < event.target.files.length; index++) {
        var ImageFile = event.target.files[index];
       setImage(ImageFile);

    }
    ImageFile.arrayBuffer()
    .then(res=>{
      const base64String = btoa(String.fromCharCode(...new Uint8Array(res)));
      const imageString=`data:image/png;base64,${base64String}`;
      
      
      setImageUrl(imageString);
      });     
  }

  const changeCompany = (event)=> {
    setCompany(event.target.value)
  }

  const changeAmmount = (event)=> {
    setAmmount(event.target.value);
  }

  const changeDate = (event)=> {
    setExpireDateStr(event.target.value.toString());
  }

  const changeSerial = (event)=> {
    setSerialNumber(event.target.value);
  }

  const cleanForm = ()=>{
    setPopupOpen(false);
    setCompany("");
    setAmmount(0);
    setExpireDateStr("");
    setSerialNumber("");
    setImage(null);
    setImageUrl("");
    setFinished(false);

  }
  
 
  const isUnique =()=>{
    let empty = myDetails.couponsList.filter(coupon=> coupon.company === company);
    empty = empty.filter(coupon=> coupon.serialNumber === serialNumber);
  
    if(empty.length !== 0){
      alert("This coupon wad already added ...")
      return false;
    } 
    return true;
  }

  
  const handleSubmit =  (event) => {
    setIsLoading(true)
    event.preventDefault();
    
    if(
      !isUnique() ||
      !userValidateDetails()

    ){
      return;
    }

    formdata.append("creator",GlobalUser.user.uid);
    formdata.append("company",company);
    formdata.append("ammount",ammount);
    formdata.append("expireDateStr",expireDateStr);
    formdata.append("serialNumber",serialNumber);
    formdata.append("Image",Image);
    
    fetch(`${CouponsBaseUrl}/newcoupon`
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
  .then(res=>res.json())
  .then((data)=>{
    setIsLoading(false)
    myDetails.couponsList= [...myDetails.couponsList,data];
    

    setPopupOpen(true);
    
  })

  }

    return (
      <Details.Consumer>
        {(myDetails)=>
        

      <div>
        <Popup 
        on='click'
        open={popupOpen}
        onOpen={()=>setPopupOpen(true)}
        >
          <div  style={styles.popup}>
            ?הקופון נוסף בהצלחה, נמשיך לעוד אחד
          </div> 
          <div>
            <Button style={styles.button} onClick={()=>setFinished(true)}  >לא</Button>
            <Button style={styles.button} onClick={cleanForm}   >כן</Button>
          
          </div>
        </Popup>

        {finished && (
          <Navigate to="/" replace={true} />
        )}

      {(!isLoading)&&
      <form onSubmit={handleSubmit} style={styles.newCoupon}>
        Company Name:
        <input type="text" value={company} maxLength={25}  style={styles.formline} onChange={changeCompany} />
        Ammount:
        <input type="number" value={ammount} min="0" max="10000" style={styles.formline} onChange={changeAmmount} />
        Expire Date:
        <input type="date" min={getTodayString()} value={expireDateStr} style={styles.formline} onChange={changeDate} />
        Coupon Serial Number:
        <input type="text" value={serialNumber} style={styles.formline} onChange={changeSerial} />
        Image:
        
        <input type="file"  onChange={changeImage} style={{display:"none"}}  src={imageUrl}  width="48" height="48" />

        <input type="file" name="uploadfile" id="img" onChange={changeImage} style={{display:"none"}} src={imageUrl} width="48" height="48"/>
        <div style={styles.formline}>
        <label htmlFor="img" style={styles.uploadImage}>Upload image</label>
        </div>

        <br/>
        <img src={imageUrl} alt="" width="200" height={imageUrl==="" ? "20" : "200"} style={styles.formline}/>
        
        <div style={styles.submit}>
        <input type="submit" value="Submit"  />

            
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

export default NewCoupon;

