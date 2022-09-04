import React, { useContext, useState } from 'react';
import Popup from 'reactjs-popup';
import 'reactjs-popup/dist/index.css';
import styles from '../Style';
import { Button } from 'react-bootstrap';
import { CouponsBaseUrl } from '../App';
import { Details } from './Table/Table';
import ReactLoading from 'react-loading';

function DeleteCoupon(props) {

  const [isOpen,setOpen] = useState(false);
  const [isLoading,setIsLoading] = useState(false);
  const myDetails=useContext(Details);

  const deleteCoupon = () => {

    setOpen(false);
    setIsLoading(true)
    let formdata = new FormData();
    formdata.append("id",myDetails.couponsList[props.index].id)
    formdata.append("creator",myDetails.user.uid)
    fetch(`${CouponsBaseUrl}`
    , {
      method: 'delete',
      mode: 'cors',
      'Access-Control-Allow-Origin': "*",
      headers: {
      'Accept': 'application/json, text/plain,multipart/form-formdata',
      },
      body: formdata
      
    })
    .then((_)=>{
      myDetails.couponsList = myDetails.couponsList.filter(coupon=> coupon.id!==myDetails.couponsList[props.index].id);
      myDetails.setList(myDetails.couponsList)      
      setIsLoading(false)
    })
    
    .catch((_)=>{
      alert("something went wrong, lets refresh and try again");
      window.location.reload(false);
    })


    }
    if(isLoading){
      return(
        <div style={styles.loading}>
           <ReactLoading type="spin" color="#123456" height={30} width={30} />
        </div>
      )
    }
    else{
    return (
      <Details.Consumer>
        {(myDetails)=> (
        <div >
        <Popup 
        on='click'
        open={isOpen}
        onOpen={()=>setOpen(true)}
        trigger={<button> Delete</button>}  >

          <div  style={styles.popup}>
          Delete this coupon?
          </div> 
          <div>
            <Button style={styles.button} onClick={deleteCoupon}  >Yes</Button>
            <Button style={styles.button} onClick={()=>setOpen(false)}   >No</Button>
          
          </div>
        </Popup>
        </div>
        )}
        </Details.Consumer>
    );
        }
}

export default DeleteCoupon;