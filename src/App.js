import './App.css';
import NewCoupon from './Components/NewCoupon';
import { BrowserRouter as Router, Route, Link, Routes } from "react-router-dom";
import UserControl from './Components/UserControl';
import Table from './Components/Table/Table';
import styles from './Style';
import About from './Components/About';
import Contact from './Components/Contact';


// "https://mycouponsorganizer.azurewebsites.net/api/coupons"
export const CouponsBaseUrl = "https://mycouponsorganizer.azurewebsites.net/api/coupons"//"https://localhost:7233/api/coupons";

//convert date from form to visable date 
export const convertDate = (date)=>{

    let day = date.substring(8);
    let month = date.substring(5,7);
    let year = date.substring(0,4);
    return day+"/"+month+"/"+year;
}
export const getTodayString=()=>{
  let today= new Date();
  let intMonth=(today.getMonth()+1)
  let month=intMonth<10 ?"0"+ intMonth.toString() : intMonth
  let intDay=(today.getDate())
  let day=intMonth<10 ?"0"+ intDay.toString() : intDay
  let todayString= ""+today.getFullYear().toString()+"-"+month.toString()+"-"+day.toString()
  return todayString;
}

export const userValidateDetails = ()=>{

  return true;
}
// export const Details=createContext()

const App = () => {
    
    return (
      

      <Router>

        <div>
            {/* <Link to="/login">Login</Link> */}
            <UserControl  />
            <Link style={styles.button} to="/">Home</Link>
          
            <Link style={styles.button} to="/NewCoupon">New Coupon</Link>

            <Link style={styles.button} to="/About">About</Link>
            
            <Link style={styles.button} to="/Contact">Contact Us</Link>
            
        </div>
      <div>
        <hr />
      <Routes>

          <Route path="/" element={<Table />} />
          <Route path="/NewCoupon" element={<NewCoupon />} />
          <Route path="/login" element={<UserControl/>} />
          <Route path="/About" element={<About/>} />
          <Route path="/Contact" element={<Contact/>} />

      </Routes>
      
      </div>
    </Router>
    
    )
  
}

export default App; 