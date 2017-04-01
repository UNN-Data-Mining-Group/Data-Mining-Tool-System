#pragma once

int main();

float get_res_(void * solver, float * in);

void set_next_weights_(void * solver, int i, float * weights);

void get_next_grads_(void * solver, int i, float * grads);

void get_next_activate_(void * solver, int i, float * activate);
