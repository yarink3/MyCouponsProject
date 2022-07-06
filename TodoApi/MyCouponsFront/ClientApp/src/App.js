import React, { Component ,useEffect, useState } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';

export default function App() {
    const [users, setUsers] = useState();

    // Function to collect data
    const getApiData = async () => {
        const response = await fetch(
            "https://localhost:7226/api/todoitems"
            , {
                mode: 'no-cors',
                header: {
                    'Access-Control-Allow-Origin': '*',
                }
            })
            .then((response) =>
                response.json());

        setUsers(response);
    };

    //useEffect(() => {
    //    getApiData();
    //}, []);
    getApiData()
    return (
        <div className="app">
            {users &&
                users.map((user) => (
                    <div>
                        Id:{user.company} <div className="title">Title:{user.company}</div>
                    </div>
                ))}
        </div>
    );
}





//    class App extends Component {
//    static displayName = App.name;
//    constructor(props) {
//        super(props);
//        this.state = { forecasts: [], userCoupons: [], datas: [], loading: true };
//        this.populateCouponData();

//    }
//    async populateCouponData() {
//        const response = await fetch('https://localhost:7226/api/todoitems',
//            Headers: { 'Access-Control-Allow-Origin': "https://localhost:7226/api/todoitems" });
//        const data = await response.json();
//        console.log("hey here", data, response);
//        this.setState({ userCoupons: data, loading: false });
//    }  
    
//  render() {
//    return (
//      //<Layout>
//      //  <Routes>
//      //    {AppRoutes.map((route, index) => {
//      //      const { element, ...rest } = route;
//      //      return <Route key={index} {...rest} element={element} />;
//      //    })}
//      //  </Routes>
//      //</Layout>
//        <div>
//            <table className='table table-striped' aria-labelledby="tabelLabel">
//                <thead>
//                    <tr>
//                        <th>Date</th>
//                        <th>Temp. (C)</th>
//                        <th>Temp. (F)</th>
//                        <th>Summary</th>
//                    </tr>
//                </thead>
//                <tbody>
//                    {this.state.userCoupons.map(userCoupons =>
//                        <tr key={userCoupons.expireDate}>
//                            <td>{userCoupons.expireDate}</td>
//                            <td>{userCoupons.company}</td>
//                            <td>{userCoupons.sum}</td>
//                            <td>{userCoupons.serialNumber}</td>
//                        </tr>
//                    )}
//                </tbody>
//            </table>


//        </div>
//    );
//  }
//}
