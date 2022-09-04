import React, { createContext, useState, useEffect } from 'react';
import styles from '../../Style';
import DeleteCoupon from "../DeleteCoupon";
import { CouponsBaseUrl } from '../../App';
import TableBody from "./TableBody"
import TableHead from "./TableHead"
import EditCoupon from '../EditCoupon';
import  { auth } from "../UserControl"
import ReactLoading from 'react-loading';
import { onAuthStateChanged } from 'firebase/auth';

let stupidList= [{ 
    "company": "victory",
    "ammount": "400",
    "expireDate": "05/23/1992",
    "serialNumber": "91474352",
    "imageUrl": "https://firebasestorage.googleapis.com/v0/b/mycoupons-6058d.appspot.com/o/Users%2Fyarink3%2FCouponsList%2F%D7%A9%D7%95%D7%A4%D7%A8%D7%A1%D7%9C2123?alt=media&token=28a976b9-52b6-42df-9c79-39247ca6df14"
  },
  {
    "company": "בדיקה",
    "ammount": "4",
    "expireDate": "22/05/2022",
    "serialNumber": "91474352",
  
  } ]
  
export const Details=createContext({
    user: null,
    set: (u)=>{},
    // username : "yarink3",
    couponsList: [],
    columns: [],
    setList: ()=>{},
    companies: [],
    uid:""

  
  })
  export var GlobalUser={}
  
const Table = (props) => {
    const [couponsList,setCouponsList] = useState([])
    const [value,changeValue] = useState(false);
    const [user,setUser] = useState(null);
    const [isLoading,setIsLoading] = useState(true);

    
    
    const columns = [
        
      {
          label: 'Company',
          accessor: 'company',
          sortable: true
      },
      {
          label: 'Ammount',
          accessor: 'ammount',
          sortable: true
      },
      {
          label: 'Expire Date',
          accessor: 'expireDate',
          sortable: true
      },
      {
          label: '',
          accessor: 'editButton',
          sortable: false
      },
      {
          label: '',
          accessor: 'deleteButton',
          sortable: false
      }
  ];

  const myDetails={
    user: user,
    uid:"",
    set: (u)=>setUser(u),
    columns: columns,
    couponsList: couponsList,
    setList: setCouponsList,
    
    companies: ["שופרסל" , "ויקטורי" , "ניצת הדובדבן" , "WOLT" , "Be"]

  }
    
    useEffect(() => {
      fetch(`${CouponsBaseUrl}/${user? user.uid:"no name"}`)
      .then((res)=>res.json())
      .then((data)=>{
        setCouponsList(data)
        setIsLoading(false)
      })
      
      
    }, [setCouponsList,user,setUser,GlobalUser]);

    
  
  couponsList.map((coupon,index)=>{
      coupon["editButton"]=(<EditCoupon changeValue={changeValue} index={index} />);
                                       
      coupon["deleteButton"]=(<DeleteCoupon changeValue={changeValue} index={index} />);

      coupon["expireDate"]=coupon["expireDate"].toString().substring(0,10)
      
    });
   
        
  
  const handleSorting = (sortField, sortOrder) => {
    if (sortField) {

     const sorted = [...myDetails.couponsList].sort((a, b) => {
      return (
       a[sortField].toString().localeCompare(b[sortField].toString(), "en", {
        numeric: true,
       }) * (sortOrder === "asc" ? 1 : -1)
      );
     });
     myDetails.setList(sorted)
    }
   };
  

  onAuthStateChanged(auth,
    (newUser) => {
      setUser(newUser)
      GlobalUser.user=newUser
    }   
  );
  if(isLoading){
    return(
      <div style={styles.loading}>
         <ReactLoading type="spin" color="#123456" height={100} width={100} />
      </div>
    )
  }
  else{
  return (
    <Details.Provider value={myDetails}>
        
    <>
    <table style={styles.tableContainer}>


    <TableHead handleSorting={handleSorting}/>
    <TableBody />
    
    </table>
    </>
    
    </Details.Provider>)
        
  
  }
  };

  export default Table;