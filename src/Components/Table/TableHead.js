import React, { useContext, useState } from 'react';
import styles from '../../Style';
import { Details } from './Table';

const TableHead = ({ handleSorting  }) => {
  const myDetails = useContext(Details)

  const [sortField, setSortField] = useState("");
  const [order, setOrder] = useState("asc");

  const handleSortingChange = (accessor) => {
      const sortOrder =
       accessor === sortField && order === "asc" ? "desc" : "asc";
      setSortField(accessor);
      setOrder(sortOrder);
      handleSorting(accessor, sortOrder);
     };
     
     return (
        <Details.Consumer>
        {(myDetails)=> ( 
      <thead>
       <tr>
        {myDetails.columns.map(({ label, accessor, sortable }) => {
         return (
          <th style={styles.tableHead}
           key={accessor}
           onClick={sortable ? () => handleSortingChange(accessor) : null}
          >
           {label}
          </th>
         );
        })}
        
       </tr>
      </thead>
      )}
      </Details.Consumer> 
     );
 };


export default TableHead;