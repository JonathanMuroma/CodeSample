import React, { useState, createElement } from 'react'
import HeaderLink from './HeaderLink.js'
import './Header.css'
import cerealBowl from '../../images/cerealBowl.png'

function Header() {
    const [header, setHeader] = useState(false);

    const changeBackground = () => {
        if(window.scrollY >= 50) {
            setHeader(true);
        } else {
            setHeader(false);
        }
    };

    window.addEventListener('scroll', changeBackground);

    return (
        <div className={header ? "Header Active" : "Header"}>
            <div className="TitleContainer">
                <a href=""><h1>Cerealland</h1></a>
                <a href=""><img src={cerealBowl} alt="Cerealland"/></a>
            </div>

            <div className="HeaderLinks">
                <HeaderLink name="Home"/>
                <HeaderLink name="About me"/>
                <HeaderLink name="Projects"/> 
                <HeaderLink name="Github code"/>
            </div>
        </div>
    )
}

export default Header;