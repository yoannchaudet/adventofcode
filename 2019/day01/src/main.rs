use std::fs::File;
use std::io::{BufRead, BufReader};

fn main() {
    // Read input
    let file = File::open("inputs/input.txt").unwrap();
    let reader = BufReader::new(file);
    let masses: Vec<f32> = reader.lines().map(|line| line.unwrap().parse().unwrap()).collect();

    let mut total_fuel = 0.0;
    masses.iter().for_each(|mass| {
        let fuel = get_fuel_requirement(*mass);
        total_fuel += fuel;
    });
    println!("Total fuel (part 1) = {}", total_fuel);

    total_fuel = 0.0;
    masses.iter().for_each(|mass| {
        let fuel = get_fuel_requirement_2(*mass);
        total_fuel += fuel;
    });
    println!("Total fuel (part 2) = {}", total_fuel);
}

fn get_fuel_requirement(mass: f32) -> f32 {
    let fuel = (mass / 3.0).floor() - 2.0;
    fuel
}

fn get_fuel_requirement_2(mass: f32) -> f32 {
    let mut total_fuel = 0.0;
    let mut current_mass = mass;
    loop {
        let current_fuel = get_fuel_requirement(current_mass);
        if current_fuel <= 0.0 {
            break;
        } else {
            total_fuel += current_fuel;
            current_mass = current_fuel;
        }
    }
    total_fuel
}