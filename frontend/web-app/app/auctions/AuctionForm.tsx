"use client";
import { Button, TextInput } from "flowbite-react";
import React, { useEffect } from "react";
import { FieldValues, useForm } from "react-hook-form";
import Input from "../components/Input";
import DateInput from "../components/DateInput";
import { createAuction } from "../actions/auctionActions";
import { useRouter } from "next/navigation";

export default function AuctionForm() {
    const router = useRouter();
    const {
        control,
        handleSubmit,
        setFocus,
        formState: { isSubmitting, isValid, isDirty, errors },
    } = useForm({
        //validation happen on touch instead of after submitting
        mode: "onTouched",
    });

    //set focus on make
    useEffect(() => {
        setFocus("make");
    }, [setFocus]);

    //fieldvalues is from react form package
    async function onSubmit(data: FieldValues) {
        console.log(data);
        try {
            //pass the fieldvalues
            const res = await createAuction(data);
            if (res.error) {
                throw new Error(res.error);
            }
            router.push(`/auctions/details/${res.id}`);
        } catch (error) {
            console.log(error);
        }
    }
    return (
        //use handleSunmit for react form
        //use the 'register' to register a field to react form
        <form
            action=""
            className="flex flex-col mt-3"
            //transfer field values to onsubmit
            onSubmit={handleSubmit(onSubmit)}
        >
            <Input
                label="Make"
                name="make"
                control={control}
                rules={{ required: "Make is required" }}
            />
            <Input
                label="Model"
                name="model"
                control={control}
                rules={{ required: "Model is required" }}
            />
            <Input
                label="Color"
                name="color"
                control={control}
                rules={{ required: "Color is required" }}
            />

            <div className="grid grid-cols-2 gap-3">
                <Input
                    label="Year"
                    name="year"
                    control={control}
                    type="number"
                    rules={{ required: "Year is required" }}
                />
                <Input
                    label="Mileage"
                    name="mileAge"
                    control={control}
                    type="number"
                    rules={{ required: "Mileage is required" }}
                />
            </div>

            <Input
                label="Image URL"
                name="imageUrl"
                control={control}
                rules={{ required: "Image URL is required" }}
            />

            <div className="grid grid-cols-2 gap-3">
                <Input
                    label="Reserve Price (Enter 0 if no reserve)"
                    name="reservePrice"
                    control={control}
                    type="number"
                    rules={{ required: "Reserve Price is required" }}
                />
                <DateInput
                    label="Aution end date/time"
                    name="auctionEnd"
                    dateFormat="dd MMMM yyyy h:mm a"
                    showTimeSelect
                    control={control}
                    rules={{ required: "Auction end date is required" }}
                />
            </div>

            <div className="flex justify-between">
                <Button outline color="gray">
                    Cancel
                </Button>
                <Button
                    isProcessing={isSubmitting}
                    //so you can only submit if all field are valid
                    disabled={!isValid}
                    type="submit"
                    outline
                    color="success"
                >
                    Submit
                </Button>
            </div>
        </form>
    );
}
