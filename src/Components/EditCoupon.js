import {React, useState, useContext, useEffect} from 'react';
import Popup from 'reactjs-popup';
import 'reactjs-popup/dist/index.css';
import styles from '../Style';
import { Button } from 'react-bootstrap';
import { Details } from './Table/Table'
import { CouponsBaseUrl, getTodayString, userValidateDetails } from '../App';


function EditCoupon(props) {
  
  const myDetails = useContext(Details);
  let formdata = new FormData();
  
  const [company,setCompany] =useState(myDetails.couponsList[props.index].company);  
  const [ammount,setAmmount] =useState(myDetails.couponsList[props.index].ammount); 
  const [expireDateStr,setExpireDateStr] =useState(myDetails.couponsList[props.index].expireDate);  
  const [serialNumber,setSerialNumber] =useState(myDetails.couponsList[props.index].serialNumber);  
  const [Image,setImage] =useState(null);  
  const [imageUrl,setImageUrl] =useState(myDetails.couponsList[props.index].imageUrl);  
  const [finished,setFinished] =useState(false);  
  const [popupOpen,setPopupOpen] =useState(false);  
  // const [id,_] =useState(company + serialNumber);  

  // console.log("edit rendered")

  useEffect(() => {
    
    cleanForm();
  
  }, [myDetails.couponsList]);


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
    setCompany(myDetails.couponsList[props.index].company);
    setAmmount(myDetails.couponsList[props.index].ammount);
    setExpireDateStr(myDetails.couponsList[props.index].expireDate);
    setSerialNumber(myDetails.couponsList[props.index].serialNumber);
    setImage(null);
    setImageUrl(myDetails.couponsList[props.index].imageUrl);
    setFinished(false);

  }

  const handleSubmit = (event) => {
    event.preventDefault();
    
    if( !userValidateDetails() ){
      return;
    }
    
    setPopupOpen(false)
    const state ={company,ammount,expireDateStr,serialNumber,Image}
    const keys=Object.keys(state)
    formdata.append("id",myDetails.couponsList[props.index].id);
    formdata.append("company",company.toString()!== myDetails.couponsList[props.index].company.toString() ? company.toString() : "");
    formdata.append("ammount", ammount !== myDetails.couponsList[props.index].ammount ? ammount : -1 );
    formdata.append("expireDateStr",expireDateStr!==  myDetails.couponsList[props.index].expireDate ? expireDateStr : "");
    formdata.append("serialNumber",serialNumber!== myDetails.couponsList[props.index].serialNumber ? serialNumber : "");
    formdata.append("Image",imageUrl!== myDetails.couponsList[props.index].imageUrl ? Image : null);

    fetch(`${CouponsBaseUrl}`,
    {
      method: 'put',
      mode: 'cors',
      'Access-Control-Allow-Origin': true,
      headers: {
      'Accept': 'application/json, text/plain,multipart/form-formdata',
      'Authorization': 'Bearer ' + sessionStorage.tokenKey
      },
      body: formdata
      
  })
  .then((res)=>{
    // set the new details in the client side
    
    keys.map((key)=>{
      let stringKey=key.toString();
      if(state[stringKey] !== myDetails.couponsList[props.index][stringKey] && state[stringKey]!== null)
        myDetails.couponsList[props.index][stringKey]=state[stringKey]
    })
    
    props.changeValue(new Date() );
    alert("קופון עודכן בהצלחה")
    return res.json()
  })
  .then((data)=>{
    myDetails.couponsList[props.index]["imageUrl"] = data.imageUrl;
    setImageUrl(data.imageUrl)
    myDetails.setList(myDetails.couponsList)
    setFinished(!finished);
    props.changeValue(new Date() );
    cleanForm()
  });
  }

  
  return (
    
    <Details.Consumer>
      {(myDetails)=> 
      (
      <Popup
      
      open={popupOpen}
      onOpen={()=>setPopupOpen(true)}
      onClose={()=>cleanForm()}
      trigger={<Button> Use / Edit</Button>}
      style={styles.editCoupon} 
      >
    
        {popupOpen &&
        <form onSubmit={handleSubmit} style={styles.editCoupon}>
        Set the changes:<br/>
        Company Name:
        <input type="text" value={company} maxLength={25}  style={styles.formline} onChange={changeCompany} />
        {/* <select  value={company} style={styles.formline} onChange={changeCompany}>
          {myDetails.companies.map((x)=><option value={x.toString()}>{x}</option>)}
        </select> */}


        Ammount Left:
        <input type="number" value={ammount} min="0" max="10000" style={styles.formline} onChange={changeAmmount} />
        Expire Date:
        <input type="date" min={getTodayString()} value={expireDateStr} style={styles.formline} onChange={changeDate} />
        Coupon Serial Number:
        <input type="text" value={serialNumber} style={styles.formline} onChange={changeSerial} />
        Image:
        <input type="file" name="uploadfile" id="img" onChange={changeImage} style={{display:"none"}} src={myDetails.couponsList[props.index].imageUrl} width="48" height="48"/>
        <div style={styles.formline}>
        <label htmlFor="img" style={styles.uploadImage}>Upload image</label>
        </div>

        <br/>
        <img src={imageUrl} alt="" width="200" height={imageUrl==="" ? "20" : "200"} style={styles.formline}/>
        
        <div style={styles.submit}>
        <input type="submit" value="Save"  />
        </div>
        </form>}
 
      </Popup>
      )}
      </Details.Consumer>
      // </div>
  );
}

export default EditCoupon;