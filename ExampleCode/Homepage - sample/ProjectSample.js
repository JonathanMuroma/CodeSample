import React from 'react'

function ProjectSample(props) {
    return(
        <div className="ProjectSampleBackground" id={props.item.class + "Background"}>
            <div className="ProjectSample" id={props.item.class}>
                <div className="ProjectTitleLink">
                    <a href="">
                        <h2 style={{display : props.item.logo == null ? "block" : "none"}}>{props.item.title}</h2>
                        <img style={{display : props.item.logo == null ? "none" : "inline-block"}} 
                        src={props.item.logo} alt={props.item.title}/>
                    </a>
                </div>
                    
                
                <div className="ProjectFlexContainer">
                    <p>{props.item.text}</p>
                    <img src={props.item.img} alt="Image here" style={{display : props.item.img == null ? "none" : "block"}}/>
                    <iframe style={{display: props.item.video == null ? "none" : "block"}}          
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