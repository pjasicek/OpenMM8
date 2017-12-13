var speed = 0.5;
var fwd : Vector3;
var targetRotation : Quaternion;

function Update ()
{
	  transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0); 
}

function LateUpdate () 
{ 	
	fwd = transform.forward;
	
	//transform.position += fwd * 0.5 * Time.deltaTime;
 
 var hit:RaycastHit;
 // instead of -Vector3.up you could use -transform.up but as hit point will jump
 // when slope changes it will give jitter. That's solvable as well by working from
 // a pivot point in bottom centre of object instead of centre (and to make sure
 // your raycast won't be too low move start pos back by a bit using again
 // transform.up as direction.
 if (Physics.Raycast(transform.position, -Vector3.up, hit, 5f)){
 
     if ( hit.distance > 1f){
             transform.localPosition.y -= hit.distance-0.1;
     } else if ( hit.distance < 1f){
             transform.localPosition.y += 0.1-hit.distance;
     }
 
     var proj : Vector3 = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
     targetRotation = Quaternion.LookRotation(proj, hit.normal);
 }
 

  
 }