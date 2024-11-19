shader_type canvas_item;

// CC0: Saturday weirdness
//  I saw a tweet from Kali where I thought I understood how he did something cool.
//  Turns out I didn't understand . Instead ended up with weird stuff.

#define iResolution 1.0/SCREEN_PIXEL_SIZE
#define fragColor COLOR
#define iTime        TIME
#define PI          3.141592654
#define TAU         (2.0*PI)

#define ROT(a)      mat2(vec2(cos(a), sin(a)), vec2(-sin(a), cos(a)))

uniform sampler2D iChannel0;
uniform vec3 ro_ = vec3(0.0, 0.5, -3.0);
uniform vec3 la_ = vec3(0.0);
uniform vec3 cube_color = vec3(1.0, 2., 3.);
uniform float cube_size = 1.0;
uniform float surface_texture = 1.0;
uniform float mask = 1.0;
uniform vec3 background = vec3(1.0, 1., 1.);

// CC0: Saturday weirdness
//  I saw a tweet from Kali where I thought I understood how he did something cool.
//  Turns out I didn't understand . Instead ended up with weird stuff.

// License: WTFPL, author: sam hocevar, found: https://stackoverflow.com/a/17897228/418488
const vec4 hsv2rgb_K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
vec3 hsv2rgb(vec3 c) {
  vec3 p = abs(fract(c.xxx + hsv2rgb_K.xyz) * 6.0 - hsv2rgb_K.www);
  return c.z * mix(hsv2rgb_K.xxx, clamp(p - hsv2rgb_K.xxx, 0.0, 1.0), c.y);
}
// License: WTFPL, author: sam hocevar, found: https://stackoverflow.com/a/17897228/418488
//  Macro version of above to enable compile-time constants
#define HSV2RGB(c)  (c.z * mix(hsv2rgb_K.xxx, clamp(abs(fract(c.xxx + hsv2rgb_K.xyz) * 6.0 - hsv2rgb_K.www) - hsv2rgb_K.xxx, 0.0, 1.0), c.y))

#define TOLERANCE           0.0001
uniform float MAX_RAY_LENGTH = 10.0;
uniform int MAX_RAY_MARCHES = 80;
#define NORM_OFF            0.005

uniform vec3 sun_color = vec3(0.6, 0.95, 1E-2);
uniform vec3 sun_diraction = vec3(0., 0., 1.0);

uniform vec3 light_color0 = vec3(.7, .85, 1.);
uniform vec3 light_position0 = vec3(4.0, 4.0, -8.0);

uniform vec3 light_cube_color = vec3(0.8, 0.75, 1.0);
uniform vec3 light_position1 = vec3(-2., -2., -4.);

uniform vec3 bottom_box_color = vec3(0.7, 0.8, 0.5);
uniform vec3 top_box_color = vec3(0.57, 0.90, 1.0);

// License: MIT, author: Inigo Quilez, found: https://iquilezles.org/articles/noacos/
mat3 rot(vec3 d, vec3 z) {
  vec3  v = cross( z, d );
  float c = dot( z, d );
  float k = 1.0/(1.0+c);

  return mat3( vec3(v.x*v.x*k + c,     v.y*v.x*k - v.z,    v.z*v.x*k + v.y),
               vec3(v.x*v.y*k + v.z,   v.y*v.y*k + c,      v.z*v.y*k - v.x),
               vec3(v.x*v.z*k - v.y,   v.y*v.z*k + v.x,    v.z*v.z*k + c)    );
}


// License: Unknown, author: Matt Taylor (https://github.com/64), found: https://64.github.io/tonemapping/
vec3 aces_approx(vec3 v) {
  v = max(v, 0.0);
  v *= 0.6f;
  float a = 2.51f;
  float b = 0.03f;
  float c = 2.43f;
  float d = 0.59f;
  float e = 0.14f;
  return clamp((v*(a*v+b))/(v*(c*v+d)+e), 0.0f, 1.0f);
}

// License: MIT, author: Inigo Quilez, found: https://iquilezles.org/articles/intersectors/
float rayPlane(vec3 ro, vec3 rd, vec4 p) {
  return -(dot(ro,p.xyz)+p.w)/dot(rd,p.xyz);
}

// License: MIT, author: Inigo Quilez, found: https://iquilezles.org/articles/distfunctions/
float box(vec2 p, vec2 b) {
  vec2 d = abs(p)-b;
  return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}




float sphere4(vec3 p, float r) {
  p*=p;
  return pow(dot(p,p), 0.25) -r;
}

float df(vec3 p, mat3 g_rot) {
  vec3 op = p;
  p *= g_rot;
  return sphere4(p, cube_size);
}

#define BACKSTEP
float rayMarch(vec3 ro, vec3 rd, float tinit, out int iter, mat3 g_rot) {
  float t = tinit;
  const float tol = TOLERANCE;
#if defined(BACKSTEP)
  vec2 dti = vec2(1e10,0.0);
#endif
  int i = 0;
  for (int i = 0; i < MAX_RAY_MARCHES; ++i) {
    float d = df(ro + rd*t, g_rot);
#if defined(BACKSTEP)
    if (d<dti.x) { dti=vec2(d,t); }
#endif
    if (d < TOLERANCE || t > MAX_RAY_LENGTH) {
      break;
    }
    t += d;
  }
#if defined(BACKSTEP)
  if(i==MAX_RAY_MARCHES) { t=dti.y; };
#endif
  iter = i;
  return t;
}


vec3 normal(vec3 pos, mat3 g_rot) {
  vec2  eps = vec2(NORM_OFF,0.0);
  vec3 nor;
  nor.x = df(pos+eps.xyy, g_rot) - df(pos-eps.xyy, g_rot);
  nor.y = df(pos+eps.yxy, g_rot) - df(pos-eps.yxy, g_rot);
  nor.z = df(pos+eps.yyx, g_rot) - df(pos-eps.yyx, g_rot);
  return normalize(nor);
}

vec3 render0(vec3 ro, vec3 rd) {
  vec3 col = vec3(0.0);

  vec3 ld0 = normalize(light_position0-ro);
  vec3 ld1 = normalize(light_position1-ro);

  float tp0  = rayPlane(ro, rd, vec4(vec3(0.0, -1.0, 0.0), -5.0));
  float tp1  = rayPlane(ro, rd, vec4(vec3(0.0, -1.0, 0.0), 6.0));

  if (tp0 > 0.0) {
    col += HSV2RGB(bottom_box_color)*exp(-0.5*(length((ro + tp0*rd).xz)));
  }

  if (tp1 > 0.0) {
    vec3 pos  = ro + tp1*rd;
    vec2 pp = pos.xz;
    float db = box(pp, vec2(5.0, 9.0))-3.0;

    col += HSV2RGB(top_box_color)*rd.y*rd.y*smoothstep(0.25, 0.0, db);
    col += 0.2*HSV2RGB(top_box_color)*exp(-0.5*max(db, 0.0));
    col += 0.05*sqrt(HSV2RGB(top_box_color))*max(-db, 0.0);
  }

  col += 1E-2*HSV2RGB(light_color0)/(1.002-dot(ld0, rd));
  col += 2E-2*HSV2RGB(light_cube_color)/(1.005-dot(ld1, rd));
  col += HSV2RGB(sun_color) / (1.001-dot(normalize(sun_diraction), rd));
  return col;
}

vec3 render1(vec3 ro, vec3 rd, mat3 g_rot, vec2 iRes) {

  int ii;
  float t = rayMarch(ro, rd, 0., ii, g_rot);

  vec3 col = render0(ro, rd * background);

  if (t < MAX_RAY_LENGTH) {
    vec3 p = ro+rd*t * surface_texture;
    vec3 n = normal(p, g_rot);
    vec3 r = reflect(rd, n);
    vec3 rcol = render0(p, r);
    float fre = 1.0+dot(rd,n);
    fre *= fre;
    fre = mix(0.5, 1.0, fre);
    vec3 ld0 = normalize(light_position0-p);
    vec3 ld1 = normalize(light_position1-p);

    float dif0 = pow(max(dot(ld0, n), 0.), 4.0)*0.5;
    float dif1 = pow(max(dot(ld1, n), 0.), 4.0)*0.5;

    col = vec3(0.);
    col += dif0*HSV2RGB(light_color0);
    col += dif1*HSV2RGB(light_cube_color);
    col += rcol*fre;
    vec2 p2 = p.xy*n.z+p.xz*n.y+p.zy*n.x;
    p2 *= 1.-0.3;
    p2 *= ROT(-20.*length(p2));
    p2.x *= iRes.y/iRes.x;
    vec2 tp = 0.5+0.5*p2;

    vec4 pcol = texture(iChannel0, tp);
    col += smoothstep(vec3(0.2, 0.25, 0.5), vec3(1.75, 1.6, 1.4), pcol.xyz);
  }

  return col;
}

void fragment() {
  mat3 g_rot;
  vec2 q = SCREEN_UV;
  vec2 p = -1. + 2. * q;
  p.x /= SCREEN_PIXEL_SIZE.x / SCREEN_PIXEL_SIZE.y;

  float a = TIME*0.25;
  vec3 r0 = vec3(1.0, sin(vec2(sqrt(0.5), 1.0)*a));
  vec3 r1 = vec3(cos(vec2(sqrt(0.5), 1.0)*a), 1.0);
  g_rot = rot(normalize(r0), normalize(r1));

  const vec3 up = vec3(0., 1., 0.);
  //vec3 ro   = vec3(0.0, 0.5, -3.0);
  vec3 ro   = ro_;
  //vec3 la   = vec3(0.0);
  vec3 la   = la_;

  vec3 ww = normalize(la-ro);
  vec3 uu = normalize(cross(up, ww));
  vec3 vv = cross(ww, uu);
  vec3 rd = normalize(p.x*uu + p.y*vv + 2.*ww);

  vec3 col = vec3(0.0);
  col = render1(ro, rd, g_rot, iResolution);
  col -= 4E-2*cube_color.zyx*(length(p)+0.25) * mask;
  col = aces_approx(col);
  col = sqrt(col);

  fragColor = vec4(col, 1.0);
}
