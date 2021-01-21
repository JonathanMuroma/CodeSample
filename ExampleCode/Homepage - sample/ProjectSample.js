import React, { useEffect } from 'react'
import Aos from 'aos';
import 'aos/dist/aos.css';

function ProjectSample(props) {
    useEffect(() => {
        Aos.init({duration: 1000});
    }, []);    
    return(
            <div className="ProjectSampleBackground" id={props.item.class + "Background"}>
                <div className="ProjectSample" id={props.item.class}>
                    <div className="ProjectTitleLink">
                        <a data-Aos="zoom-in" href="">
                            <h2 style={{display : props.item.logo == null ? "block" : "none"}}>{props.item.title}</h2>
                            <img style={{display : props.item.logo == null ? "none" : "inline-block"}} 
                            src={props.item.logo} alt={props.item.title}/>
                        </a>
                    </div>
                    
                
                    <div className="ProjectFlexContainer">
                        <p data-Aos="fade">{props.item.text}</p>
                        <img data-Aos="fade" src={props.item.img} alt="Image here" style={{display : props.item.img == null ? "none" : "block"}}/>
                        <iframe data-Aos="fade" style={{display: props.item.video == null ? "none" : "block"}}          
                        src={props.item.video} 
                        frameBorder="0"
                        allowFullScreen>
                        </iframe>
                    </div> 
                </div>
            </div>
    );
}

export default ProjectSample;