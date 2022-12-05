import React from 'react'
import catData from './catData.js'
import CatInfo from './CatInfo.js'
import CatNumberListing from './CatNumberListing'

class CatHandler extends React.Component {
    constructor() {
        super();
        this.state = {
            cat : catData,
            counter : 0,
            searchInput : 1}
        
        this.nextCatInfo = this.nextCatInfo.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.searchCat = this.searchCat.bind(this);
        this.setCounter = this.setCounter.bind(this);
    }

    nextCatInfo(num) {
        //event.preventDefault();
        if(num > 0) {
            if(this.state.counter < this.state.cat.length - 1) {
                this.setState(prevState => ({counter : prevState.counter+1}))
            } else {
                this.setState({counter : 0})
            }
        } else {
            if(this.state.counter > 0) {
                this.setState(prevState => ({counter : prevState.counter-1}))
            } else {
                this.setState({counter : 11})
            }
        }
    }

    searchCat(num) {
        if(num >= 1 && num <= this.state.cat.length) {
            this.setState({counter : num-1})
        } else {
            alert("Give number between 1 or " + this.state.cat.length);
        }
    }

    handleChange(event) {
        const {name, value} = event.target;
        this.setState({[name] : value})
    }

    setCounter(num) {
        this.setState({counter : num})
    }

    render() {
        //console.log(this.state.counter)
        return (
            <div>
                <a href={this.state.cat[this.state.counter].image} >
                    <img src={this.state.cat[this.state.counter].image} alt="Cat image" height="250"/>
                </a>
                <p>Name: {this.state.cat[this.state.counter].name}, age: {this.state.cat[this.state.counter].age}</p>
                <p>Quote "{this.state.cat[this.state.counter].quote}"</p>       
                <CatNumberListing current={this.state.counter} length={this.state.cat.length} setCounter={this.setCounter}/>
                <button onClick={() =>this.nextCatInfo(-1)} >Prev</button>
                <button onClick={() =>this.nextCatInfo(1)} >Next</button>
                <div>
                    <input type="number" name="searchInput" value={this.state.searchInput} onChange={this.handleChange}/>
                    <button onClick={() => this.searchCat(this.state.searchInput)}>Search</button>
                </div>
                
                <p>{this.state.counter+1}/{this.state.cat.length}</p> 
            </div>
        );
    }
}

export default CatHandler;