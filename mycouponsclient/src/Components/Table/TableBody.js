import styles from '../../Style';
import { Details } from './Table';
import { convertDate } from '../../App';

const TableBody = () => {
    
    return (
        <Details.Consumer>
            {(myDetails)=> 
            ( 
        <tbody >
        {myDetails.couponsList.map((data) => {
        return (
        
        <tr  style={styles.tableLine}
            key={data.id}>
            {myDetails.columns.map(({ accessor }) => {
            var tData;
            
            if(accessor.toString() === "expireDate"){
                tData = convertDate(data[accessor]);
            }
            else{
                tData = data[accessor] ? data[accessor] : "——";
            }
            return <td     
            key={accessor}>{tData}</td>;
            })}
        </tr>
        );
        })}
        </tbody>
        )}
    </Details.Consumer>
    );
  };

  export default TableBody;